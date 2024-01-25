using System;
using System.Collections.Generic;
using UnityEngine;

public class FamilyTreeView : MonoBehaviour
{
    private FamilyTreeController controller = new FamilyTreeController();
    public FamilyTreeController Controller => controller;

    [SerializeField] private GameObject nodePrefab = null;
    
    [Serializable]
    public struct NodeParameters
    {
        public Vector2 childDistance;
        public Vector2 spouseDistance;
        [Range(-1, 1)] public int spouseDirection; // right: +1, left: -1
        [Range(-1, 1)] public int childDirection; // up: +1, down: -1
    }

    [SerializeField] private NodeParameters nodeParams;
    public NodeParameters NodeParams => nodeParams;
    
    private Dictionary<int, FamilyTreeNode> nodes = new Dictionary<int, FamilyTreeNode>(); 
    private Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();

    private List<int> deactiveNodes = new List<int>();

    private void Awake()
    {
        controller.RegisterAddPersonListener(CreateNode);
        controller.RegisterAddPersonListener(i => Refresh());
        controller.RegisterAddFamilyListener(i => Refresh());
        controller.RegisterAddChildListener((i, j) => Refresh());
        
        if(nodePrefab == null)
            Debug.LogError("Node prefab is not set.");
    }

    private void CreateNode(int id)
    {
        FamilyTreeNode node = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity).GetComponent<FamilyTreeNode>();
        node.Initialize(id);
        nodes.TryAdd(id, node);
    }
    
    private void Refresh()
    {
        PlaceNodes();
    }

    private void PlaceNodes()
    {
        PersonPool people = controller.People;
        positions.Clear();
        ResetDeactiveNodes();
        
        for (int i = 0; i < people.Pool.Count; i++)
            CalculateNodePosition(i);
        
        for (int i = 0; i < people.Pool.Count; i++)
        {
            if (!positions.ContainsKey(i))
            {
                deactiveNodes.Add(i);
                nodes[i].gameObject.SetActive(false);
                continue;
            }
            
            nodes[i].transform.position = positions[i];
        }
    }

    private void ResetDeactiveNodes()
    {
        foreach (int id in deactiveNodes)
            nodes[id].gameObject.SetActive(true);
        
        deactiveNodes.Clear();
    }

    private int FindIndexInList(int id, in List<PersonID> ids)
    {
        for (int i = 0; i < ids.Count; i++)
            if (ids[i].Value == id)
                return i;

        return -1;
    }

    private void CalculateNodePosition(int id)
    {
        if (positions.ContainsKey(id)) return;
        if (!controller.TryGetFamiliesInvolved(id, out var families)) return;
        
        Family ownerOf = controller.LookupFamily(families.OwnerOf);
        Family childOf = controller.LookupFamily(families.ChildOf);

        if (positions.Count == 0)
            positions[id] = Vector2.zero;
        else
        {
            if (ownerOf != null)
            {
                int otherParent = (ownerOf.Mother.Value == id) ? ownerOf.Father.Value : ownerOf.Mother.Value;
                int direction = (ownerOf.Mother.Value == id) ? -1 : 1;

                if (positions.TryGetValue(otherParent, out var opPos))
                    positions[id] = opPos + new Vector2(nodeParams.spouseDistance.x * nodeParams.spouseDirection * -direction, 0);
                else
                {
                    int count = ownerOf.Children.Count;

                    for (int i = 0; i < count; i++)
                    {
                        if (!positions.TryGetValue(ownerOf.Children[i].Value, out Vector2 cPos)) continue;

                        positions[id] = cPos + new Vector2(-((i - 0.5f * (count - 1)) + 0.5f * direction) * nodeParams.childDistance.x,
                            nodeParams.childDistance.y * nodeParams.childDirection);
                        break;
                    }
                }
            }

            if (childOf != null)
            {
                if (positions.TryGetValue(childOf.Father.Value, out var fPos))
                {
                    controller.TryGetFamiliesInvolved(childOf.Father, out var fFamilies);
                    var children = controller.LookupFamily(fFamilies.OwnerOf).Children;
                    int i = FindIndexInList(id, in children);

                    float childOffset = i * nodeParams.childDistance.x;
                    float childOffsetNormal = childOffset - (children.Count - 1) * 0.5f * nodeParams.childDistance.x;
                    childOffsetNormal += 0.5f * nodeParams.spouseDistance.x;
                    
                    
                    if (i != -1)
                        positions[id] = fPos + new Vector2(childOffsetNormal,
                            nodeParams.childDistance.y * nodeParams.childDirection);
                }
                else if (positions.TryGetValue(childOf.Mother.Value, out var mPos))
                {
                    controller.TryGetFamiliesInvolved(childOf.Mother, out var fFamilies);
                    var children = controller.LookupFamily(fFamilies.OwnerOf).Children;
                    int i = FindIndexInList(id, in children);
                    
                    float childOffset = i * nodeParams.childDistance.x;
                    float childOffsetNormal = childOffset - (children.Count - 1) * 0.5f * nodeParams.childDistance.x;
                    childOffsetNormal += 0.5f * nodeParams.spouseDistance.x;
                    
                    if (i != -1)
                        positions[id] = mPos + new Vector2(childOffsetNormal,
                            nodeParams.childDistance.y * nodeParams.childDirection);
                }
            }
        }


        //Spouse First
        if (ownerOf != null)
        {
            int otherParent = (ownerOf.Mother.Value == id) ? ownerOf.Father.Value : ownerOf.Mother.Value;
            CalculateNodePosition(otherParent);
        }
        
        //Parents Second
        if (childOf != null)
        {
            CalculateNodePosition(childOf.Father.Value);
            CalculateNodePosition(childOf.Mother.Value);
        }
        
        //Children Third
        if (ownerOf != null)
        {
            foreach (PersonID child in ownerOf.Children)
                CalculateNodePosition(child.Value);
        }
    }
}
