using System;

[Serializable]
public struct FamilyID : IID
{
    private int value;
    public int Value => value;

    public FamilyID(int value) => this.value = value;
    public bool Equals(IID id) => Value == id.Value;
}
