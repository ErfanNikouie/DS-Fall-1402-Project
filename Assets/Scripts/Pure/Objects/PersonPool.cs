using System;
using System.Collections.Generic;

[Serializable]
public class PersonPool : IPool<Person>
{
    private List<Person> pool = new List<Person>();
    public List<Person> Pool => pool;
    
    public bool Add(Person value)
    {
        if (Pool.Contains(value)) return false;
        
        Pool.Add(value);
        return true;
    }
	
    public bool ValidateID(IID id) => id.Value < Pool.Count && id.Value >= 0;
    public IID GenerateUniqueID() => new PersonID(Pool.Count);
    public Person Lookup(IID id) => !ValidateID(id) ? null : Pool[id.Value];
}
