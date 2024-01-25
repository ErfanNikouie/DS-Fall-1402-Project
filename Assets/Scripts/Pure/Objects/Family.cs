using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands.TubeClient;

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

    public bool AddChild(PersonID person)
    {
        if (children.Contains(person)) return false;
        
        children.Add(person);
        return true;
    }

    public bool IsEqual(Family family)
    {
        if (!father.Equals(family.father) || !mother.Equals(family.mother) || children.Count != family.Children.Count)
            return false;
        
        for (int i = 0; i < children.Count; i++)
            if (!children[i].Equals(family.Children[i]))
                return false;

        return true;
    }
}
