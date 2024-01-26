using UnityEngine;
using QFSW.QC;
using QFSW.QC.Utilities;
using UnityEngine.SceneManagement;

public class ConsoleCommand : MonoBehaviour
{
    [SerializeField] private FamilyTreeView ftv;
    [SerializeField] private FamilyTreeMock mock;

    [SerializeField] private Color success = Color.green;
    [SerializeField] private Color warning = Color.yellow;
    [SerializeField] private Color error = Color.red;
    
    [Command("add-person", "Adds a person by name. Name will be hashed and stored in the object.")]
    public void AddPerson(string name)
    {
        Person person = new Person(name);
        PersonID pid = ftv.Controller.GetUniquePersonID();
        bool result = ftv.Controller.AddPerson(person);
        
        if(result)
            Debug.Log($"Person with ID: '{pid.Value}' has been added. Hash: '{person.Name}'.".ColorText(success));
        else
            Debug.Log("Person could not be added.".ColorText(error));
    }
    
    [Command("add-family", "Adds a family by its father and mother IDs.")]
    public void AddFamily(int fatherID, int motherID)
    {
        PersonID father = new PersonID(fatherID);
        PersonID mother = new PersonID(motherID);
        Family family = new Family(father, mother);
        FamilyID fid = ftv.Controller.GetUniqueFamilyID();
        bool result = ftv.Controller.AddFamily(family);
        
        if(result)
            Debug.Log($"Family with ID: '{fid.Value}' has been added.".ColorText(success));
        else
            Debug.Log("Family could not be added.".ColorText(error));
    }
    
    [Command("add-child", "Adds a child (by its ID) to a family (by its ID).")]
    public void AddChild(int childID, int familyID)
    {
        PersonID child = new PersonID(childID);
        FamilyID family = new FamilyID(familyID);
        bool result = ftv.Controller.AddChildToFamily(child, family);
        
        if(result)
            Debug.Log($"Child with ID: '{child.Value}' has been added to the family with ID: '{family.Value}'.".ColorText(success));
        else
            Debug.Log("Child could not be added.".ColorText(error));
    }
    
    [Command("check-parent", "Checks if the 'parentID' is the parent of 'childID'.")]
    public void CheckParentRelation(int parentID, int childID)
    {
        PersonID parent = new PersonID(parentID);
        PersonID child = new PersonID(childID);
        bool result = ftv.Controller.CheckParentRelation(parent, child);
        
        if(result)
            Debug.Log($"Person with ID: '{parentID}' is a parent of person with ID: '{childID}'.".ColorText(success));
        else
            Debug.Log($"Person with ID: '{parentID}' is not a parent of person with ID: '{childID}'.".ColorText(warning));
    }
    
    [Command("check-parent", "Checks if the first selected node is the parent of the second node.")]
    public void CheckParentRelation()
    {
        int parentID = ftv.GetSelection(0);
        int childID = ftv.GetSelection(1);
        CheckParentRelation(parentID, childID);
    }

    [Command("check-sibling", "Checks if 'firstID' and 'secondID' are siblings (i.e. born in the same family as children).")]
    public void CheckSiblingRelation(int firstID, int secondID)
    {
        PersonID first = new PersonID(firstID);
        PersonID second = new PersonID(secondID);
        bool result = ftv.Controller.CheckSiblingRelation(first, second);
        
        if(result)
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are siblings.".ColorText(success));
        else
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are not siblings.".ColorText(warning));
    }
    
    [Command("check-sibling", "Checks if the two selected nodes are siblings (i.e. born in the same family as children).")]
    public void CheckSiblingRelation()
    {
        int firstID = ftv.GetSelection(0);
        int secondID = ftv.GetSelection(1);
        
        CheckSiblingRelation(firstID, secondID);
    }

    [Command("bfs-common-families", "Gets all common families between 'firstID' and 'secondID' with a breadth-first search.")]
    public void GetCommonFamiliesBFS(int firstID, int secondID)
    {
        PersonID first = new PersonID(firstID);
        PersonID second = new PersonID(secondID);
        var cf = ftv.Controller.GetCommonFamiliesBFS(first, second);
        
        if(cf.Count > 0)
            Debug.Log($"All '{cf.Count}' families person with ID: '{firstID}' and person with ID: '{secondID}' have in common:".ColorText(success));
        else
            Debug.Log($"No common families found for person with ID: '{firstID}' and person with ID: '{secondID}'.".ColorText(warning));

        for (int i = 0; i < cf.Count; i++)
            Debug.Log($"{i + 1}. {cf[i].Value}".ColorText(success));
    }
    
    [Command("bfs-common-families", "Gets all common families between the two selected nodes with a breadth-first search.")]
    public void GetCommonFamiliesBFS()
    {
        int firstID = ftv.GetSelection(0);
        int secondID = ftv.GetSelection(1);
        GetCommonFamiliesBFS(firstID, secondID);
    }
    
    [Command("fast-common-family", "Gets one common family between 'firstID' and 'secondID' through a custom-made and more optimized breadth-first search.")]
    public void GetCommonFamilyFast(int firstID, int secondID)
    {
        PersonID first = new PersonID(firstID);
        PersonID second = new PersonID(secondID);
        var cf = ftv.Controller.GetCommonFamilyFast(first, second);

        if(cf.Value >= 0)
            Debug.Log($"Family with ID: '{cf.Value}' was found in common for person with ID: '{firstID}' and person with ID: '{secondID}'.".ColorText(success));
        else
            Debug.Log($"No common families found for person with ID: '{firstID}' and person with ID: '{secondID}'.".ColorText(warning));
    }
    
    [Command("fast-common-family", "Gets one common family between the two selected nodes through a custom-made and more optimized breadth-first search.")]
    public void GetCommonFamilyFast()
    {
        int firstID = ftv.GetSelection(0);
        int secondID = ftv.GetSelection(1);
        
        GetCommonFamilyFast(firstID, secondID);
    }
    
    [Command("check-bfs-distant", "Checks if 'firstID' and 'secondID' have a family in common through a breadth-first search.")]
    public void CheckDistantRelationBFS(int firstID, int secondID)
    {
        PersonID first = new PersonID(firstID);
        PersonID second = new PersonID(secondID);
        bool result = ftv.Controller.CheckDistantRelationBFS(first, second);
        
        if(result)
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are distant relatives (i.e. have at least one family in common).".ColorText(success));
        else
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are not distant relatives (i.e. have no family in common).".ColorText(warning));
    }
    
    [Command("check-bfs-distant", "Checks if the two selected nodes have a family in common through a breadth-first search.")]
    public void CheckDistantRelationBFS()
    {
        int firstID = ftv.GetSelection(0);
        int secondID = ftv.GetSelection(1);

        CheckDistantRelationBFS(firstID, secondID);
    }
    
    [Command("check-fast-distant", "Checks if 'firstID' and 'secondID' have a family in common through a custom-made and more optimized breadth-first search.")]
    public void CheckDistantRelationFast(int firstID, int secondID)
    {
        PersonID first = new PersonID(firstID);
        PersonID second = new PersonID(secondID);
        bool result = ftv.Controller.CheckDistantRelationFast(first, second);
        
        if(result)
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are distant relatives (i.e. have at least one family in common).".ColorText(success));
        else
            Debug.Log($"Person with ID: '{firstID}' and person with ID: '{secondID}' are not distant relatives (i.e. have no family in common).".ColorText(warning));
    }
    
    [Command("check-fast-distant", "Checks if the two selected nodes have a family in common through a custom-made and more optimized breadth-first search.")]
    public void CheckDistantRelationFast()
    {
        int firstID = ftv.GetSelection(0);
        int secondID = ftv.GetSelection(1);
        
        CheckDistantRelationFast(firstID, secondID);
    }
    
    [Command("get-farthest-born", "Gets the farthest born of 'parentID'.")]
    public void GetFarthestBorn(int parentID)
    {
        PersonID parent = new PersonID(parentID);

        PersonID farthest = ftv.Controller.GetFarthestBorn(parent, out int distance);

        if (farthest.Value >= 0)
        {
            Debug.Log($"The farthest born of person with ID: '{parentID}' is person with ID: '{farthest.Value}' with a '{distance}' unit distance.".ColorText(success));
            ftv.ClearResultNodes();
            ftv.ResultNode(farthest.Value);
        }
        else
            Debug.Log($"No farthest born was found for person with ID: '{parentID}'.".ColorText(warning));
    }
    
    [Command("get-farthest-born", "Gets the farthest born of the selected node.")]
    public void GetFarthestBorn()
    {
        int parentID = ftv.GetSelection(0);
        GetFarthestBorn(parentID);
    }
    
    [Command("get-farthest-relation", "Gets the farthest relation available between the nodes.")]
    public void GetFarthestRelation(int rootID)
    {
        PersonID root = new PersonID(rootID);
        
        ftv.Controller.GetFarthestRelation(root, out var first, out var second, out int distance);

        if (first.Value >= 0 && second.Value >= 0)
        {
            Debug.Log($"Person with ID: '{first.Value}' and person with ID: '{second.Value}' have the farthest relation with a distance of '{distance}' unit.".ColorText(success));
            ftv.ClearResultNodes();
            ftv.ResultNode(first.Value);
            ftv.ResultNode(second.Value);
        }
        else
            Debug.Log($"Farthest relation could not be found.".ColorText(warning));
    }
    
    [Command("get-farthest-relation", "Gets the farthest relation available between the nodes.")]
    public void GetFarthestRelation()
    {
        int rootID = ftv.GetSelection(0);
        
        GetFarthestRelation(rootID);
    }
    
    [Command("search-person", "Finds the person with 'name', and displays their ID and hash.")]
    public void SearchPerson(string name)
    {
        Person p = new Person(name);
        PersonID pid = ftv.Controller.ContainsPerson(p);

        if (pid.Value >= 0)
        {
            Debug.Log($"Person with name: '{name}': ID: '{pid.Value}', Hash: '{p.Name}'.".ColorText(success));
            ftv.ClearResultNodes();
            ftv.ResultNode(pid.Value);
        }
        else
            Debug.Log($"No person with name: '{name}' was found.".ColorText(error));
    }
    
    [Command("select", "Selects the node by its 'id'.")]
    public void SelectNode(int id)
    {
        bool result = ftv.SelectNode(id);
        
        if(!result)
            Debug.Log($"Node with ID: '{id}' could not be selected.".ColorText(error));
    }
    
    [Command("deselect", "Deselects the node by its 'id'.")]
    public void DeselectNode(int id)
    {
        bool result = ftv.DeselectNode(id);
        
        if(!result)
            Debug.Log($"Node with ID: '{id}' could not be deselected.".ColorText(error));
    }

    [Command("clear-selection", "Clears the current selection.")]
    public void ClearSelection() => ftv.ClearSelectedNodes();
    
    [Command("clear-results", "Clears the current result.")]
    public void ClearResults() => ftv.ClearResultNodes();

    [Command("load-mock", "Loads a pre-determined mock graph.")]
    public void LoadMock() => mock.CreateFamily(ftv.Controller);

    [Command("reload", "Reloads the scene.")]
    public void Reload() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
