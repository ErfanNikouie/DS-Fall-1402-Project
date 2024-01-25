using System;
using UnityEngine;
using QFSW.QC;

public class ConsoleCommand : MonoBehaviour
{
    [SerializeField] private FamilyTreeView ftv;
    
    [Command("add-person", "Adds a person by name. Name will be hashed and stored in the object.")]
    public void AddPerson(string name)
    {
        Person person = new Person(name);
        PersonID pid = ftv.Controller.GetUniquePersonID();
        bool result = ftv.Controller.AddPerson(person);
        
        if(result)
            Debug.Log($"Person with ID:{pid.Value} has been added. Hash:{person.Name}.");
        else
            Debug.LogError("Person could not be added.");
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
            Debug.Log($"Family with ID:{fid.Value} has been added.");
        else
            Debug.LogError("Family could not be added.");
    }
    
    [Command("add-child", "Adds a child (by its ID) to a family (by its ID).")]
    public void AddChild(int childID, int familyID)
    {
        PersonID child = new PersonID(childID);
        FamilyID family = new FamilyID(familyID);
        bool result = ftv.Controller.AddChildToFamily(child, family);
        
        if(result)
            Debug.Log($"Child with ID:{child.Value} has been added to the family with ID:{family.Value}.");
        else
            Debug.LogError("Child could not be added.");
    }
    
    
}
