using System;
using System.Collections.Generic;

[Serializable]
public class FamilyPool : IPool<Family>
{
    private List<Family> pool = new List<Family>();
    public List<Family> Pool => pool;
    
    public void Add(Family value)
    {
        if (Pool.Contains(value)) return;
        
        Pool.Add(value);
    }
	
    public bool ValidateID(IID id) => id.Value < Pool.Count && id.Value >= 0;
    public IID GenerateUniqueID() => new FamilyID(Pool.Count);
    public Family Lookup(IID id) => !ValidateID(id) ? null : Pool[id.Value];
}
