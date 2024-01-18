using System.Collections.Generic;
using TMPro;

public class FamilyTree
{
	private PersonPool people = new PersonPool();
	private FamilyPool families = new FamilyPool();

	private Dictionary<PersonID, FamiliesInvolved> familiesInvolvedDict = new Dictionary<PersonID, FamiliesInvolved>();

	private void AddFamiliesInvolvedAsChild(PersonID person, FamilyID family)
	{
		if (!familiesInvolvedDict.ContainsKey(person))
			familiesInvolvedDict[person] = new FamiliesInvolved();
		
		familiesInvolvedDict[person].ChildOf = family;
	}
	
	private void AddFamiliesInvolvedAsOwner(PersonID person, FamilyID family)
	{
		if (!familiesInvolvedDict.ContainsKey(person))
			familiesInvolvedDict[person] = new FamiliesInvolved();

		familiesInvolvedDict[person].OwnerOf = family;
	}

	public void AddPerson(Person person) => people.Add(person);

	public void AddFamily(Family family)
	{
		if (!people.ValidateID(family.Father) || !people.ValidateID(family.Mother)) return;

		FamilyID fid = (FamilyID)families.GenerateUniqueID();
		
		AddFamiliesInvolvedAsOwner(family.Father, fid);
		AddFamiliesInvolvedAsOwner(family.Mother, fid);
		families.Add(family);
	}

	public void AddChildToFamily(PersonID child, FamilyID family)
	{
		if (!people.ValidateID(child) || !families.ValidateID(family)) return;
		
		AddFamiliesInvolvedAsChild(child, family);
		families.Lookup(family).AddChild(child);
	}

	public bool CheckParentRelation(PersonID parent, PersonID child) // if 'parent' is a parent of 'child'
	{
		if (!people.ValidateID(parent) || !people.ValidateID(child)) return false;
		if (parent.Equals(child)) return true;

		var parentFamily = families.Lookup(familiesInvolvedDict[child].ChildOf);
		return CheckParentRelation(parent, parentFamily.Father) || CheckParentRelation(parent, parentFamily.Mother);
	}

	public bool CheckSiblingRelation(PersonID first, PersonID second) // if 'first' is a sibling of 'second'
	{
		if (!people.ValidateID(first) || !people.ValidateID(second)) return false;

		var firstFamily = familiesInvolvedDict[first].ChildOf;
		var secondFamily = familiesInvolvedDict[second].ChildOf;

		return firstFamily.Equals(secondFamily);
	}

	public bool CheckDistantRelation(PersonID first, PersonID second)
	{
		if (!people.ValidateID(first) || !people.ValidateID(second)) return false;
		
		var firstFamily = familiesInvolvedDict[first].ChildOf;
		var secondFamily = familiesInvolvedDict[second].ChildOf;
		
		return firstFamily.Equals(secondFamily);
	}
}
