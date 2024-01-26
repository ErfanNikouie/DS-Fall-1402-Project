using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FamilyTreeView : MonoBehaviour
{
    private FamilyTreeController controller = new FamilyTreeController();
    public FamilyTreeController Controller => controller;

    [Header("Nodes")]
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

    [Header("Links")]
    [SerializeField] private GameObject linkPrefab = null;

    [SerializeField] private Material[] materials;
    private Material RandomMaterial => materials[Random.Range(0, materials.Length)];

    private Dictionary<int, FamilyTreeNode> nodes = new Dictionary<int, FamilyTreeNode>();
    private Dictionary<int, Vector2> positions = new Dictionary<int, Vector2>();
    private Dictionary<int, FamilyTreeLink> links = new Dictionary<int, FamilyTreeLink>();

    private List<int> deactiveNodes = new List<int>();
    private List<int> selectedNodes = new List<int>();
    private List<int> resultNodes = new List<int>();
    private List<int> deactiveLinks = new List<int>();

    private void Awake()
    {
        controller.RegisterAddPersonListener(CreateNode);
        controller.RegisterAddPersonListener(i => Refresh());
        
        controller.RegisterAddFamilyListener(CreateFamily);
        controller.RegisterAddFamilyListener(i => Refresh());
        
        controller.RegisterAddChildListener((i, j) => Refresh());

        if (nodePrefab == null)
            Debug.LogError("Node prefab is not set.");
    }

    private void CreateNode(int id)
    {
        FamilyTreeNode node = Instantiate(nodePrefab, Vector3.zero, Quaternion.identity).GetComponent<FamilyTreeNode>();
        node.Initialize(id);
        nodes.TryAdd(id, node);
    }

    private void CreateFamily(int id)
    {
        FamilyTreeLink link = Instantiate(linkPrefab, Vector3.zero, Quaternion.identity).GetComponent<FamilyTreeLink>();

        link.Initialize(RandomMaterial);
        links.TryAdd(id, link);
    }

    public void Refresh()
    {
        PlaceNodes();
        LinkNodes();
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

    private void LinkNodes()
    {
        FamilyPool families = controller.Families;
        //ResetDeactiveLinks();

        for (int i = 0; i < families.Pool.Count; i++)
        {
            Family f = families.Pool[i];

            Vector2 father = positions[f.Father.Value];
            Vector2 mother = positions[f.Mother.Value];
            Vector2[] children = new Vector2[f.Children.Count];

            for (int j = 0; j < f.Children.Count; j++)
                children[j] = positions[f.Children[j].Value];
            
            links[i].SetPoints(father, mother, children);
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

    public void ClearSelectedNodes()
    {
        int[] snodes = selectedNodes.ToArray();
        foreach (var id in snodes)
        {
            if(!nodes.TryGetValue(id, out var node)) continue;
            
            DeselectNode(id);
        }
        
        selectedNodes.Clear();
    }
    
    public bool SelectNode(int id)
    {
        if (!nodes.TryGetValue(id, out var node) && !selectedNodes.Contains(id)) return false;
        
        node.SetState(selected: true);
        selectedNodes.Add(id);
        return true;
    }

    public bool DeselectNode(int id)
    {
        if (!nodes.TryGetValue(id, out var node) && selectedNodes.Contains(id)) return false;

        node.SetState(selected: false);
        selectedNodes.Remove(id);
        return true;
    }

    public void ClearResultNodes()
    {
        int[] rnodes = resultNodes.ToArray();
        foreach (var id in rnodes)
        {
            if(!nodes.TryGetValue(id, out var node)) continue;
            
            DeresultNode(id);
        }
        
        resultNodes.Clear();
    }
    

    public bool ResultNode(int id)
    {
        if (!nodes.TryGetValue(id, out var node) && !resultNodes.Contains(id)) return false;
        
        node.SetState(result: true);
        resultNodes.Add(id);
        return true;
    }

    public bool DeresultNode(int id)
    {
        if (!nodes.TryGetValue(id, out var node) && resultNodes.Contains(id)) return false;

        node.SetState(result: false);
        resultNodes.Remove(id);
        return true;
    }

    public int GetSelection(int id)
    {
        if (selectedNodes.Count <= id) return -1;
        
        return selectedNodes[id];
    }
}