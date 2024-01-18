using System;

[Serializable]
public struct FamilyID : IID
{
    private int value;
    public int Value => value;

    public FamilyID(int value = -1) => this.value = value;
}
