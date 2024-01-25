using UnityEngine;

public class FamilyTreeView : MonoBehaviour
{
    private FamilyTreeController controller;

    private void Awake()
    {
        controller = new FamilyTreeController();
        
        controller.RegisterAddPersonListener(Refresh);
        controller.RegisterAddFamilyListener(Refresh);
        controller.RegisterAddChildListener(Refresh);
    }
    
    private void Refresh()
    {
        
    }
}
