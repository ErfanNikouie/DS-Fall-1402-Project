using System;

[Serializable]
public class FamiliesInvolved
{
    public FamilyID ChildOf; //The family which this Person is a child of
    public FamilyID OwnerOf; //The family which this Person is the owner of
}
