using System;
using System.Collections.Generic;

[Serializable]
public class PersonPool : IPool<Person>
{
    private List<Person> pool = new List<Person>();
    public List<Person> Pool => pool;
    
    public void Add(Person value)
    {
        if (Pool.Contains(value)) return;
        
        Pool.Add(value);
    }
	
    public bool ValidateID(IID id) => id.Value < Pool.Count && id.Value >= 0;
    public IID GenerateUniqueID() => new FamilyID(Pool.Count);
    public Person Lookup(IID id) => !ValidateID(id) ? null : Pool[id.Value];
}
