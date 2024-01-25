using UnityEngine;

public class FamilyTreeView : MonoBehaviour
{
    private FamilyTreeController controller = null;

    [SerializeField] private GameObject nodePrefab = null;

    private void Awake()
    {
        controller = new FamilyTreeController();
        
        controller.RegisterAddPersonListener(Refresh);
        controller.RegisterAddFamilyListener(Refresh);
        controller.RegisterAddChildListener(Refresh);
        
        if(nodePrefab == null)
            Debug.LogError("Node prefab is not set.");
    }
    
    private void Refresh()
    {
        
    }
}
