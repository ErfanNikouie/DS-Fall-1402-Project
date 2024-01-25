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
        ftv.Controller.AddPerson(person);
        
        Debug.Log($"Person with ID:{pid.Value} has been added. Hash:{person.Name}.");
    }
    
    [Command("add-family", "Adds a family by its father and mother ids.")]
    public void AddFamily(int fatherID, int motherID)
    {
        PersonID father = new PersonID(fatherID);
        PersonID mother = new PersonID(motherID);
        Family family = new Family(father, mother);
        FamilyID fid = ftv.Controller.GetUniqueFamilyID();
        ftv.Controller.AddFamily(family);
        
        Debug.Log($"Family with ID:{fid.Value} has been added.");
    }
    
    [Command("add-child", "Adds a child (by its id) to a family (by its id).")]
    public void AddChild(int childID, int familyID)
    {
        PersonID child = new PersonID(childID);
        FamilyID family = new FamilyID(familyID);
        ftv.Controller.AddChildToFamily(child, family);
        
        Debug.Log($"Child with ID:{child.Value} has been added to the family with ID:{family.Value}.");
    }
}
