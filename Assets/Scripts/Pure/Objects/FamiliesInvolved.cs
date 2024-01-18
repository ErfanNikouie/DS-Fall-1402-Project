using System;

[Serializable]
public class FamiliesInvolved
{
    public FamilyID ChildOf = new FamilyID(-1); //The family which this Person is a child of
    public FamilyID OwnerOf = new FamilyID(-1); //The family which this Person is the owner of
}
