using System;
using System.Collections.Generic;

[Serializable]
public class Family
{
    private PersonID father;
    private PersonID mother;
    private List<PersonID> children = new List<PersonID>();

    public PersonID Father => father;
    public PersonID Mother => mother;
    public List<PersonID> Children => children;

    public Family(PersonID father, PersonID mother)
    {
        this.father = father;
        this.mother = mother;
    }

    public void AddChild(PersonID person)
    {
        if (children.Contains(person)) return;
        
        children.Add(person);
    }
}
