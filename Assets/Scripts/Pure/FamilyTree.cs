using System.Collections.Generic;
using System.Linq;

public class FamilyTree
{
	private PersonPool people = new PersonPool();
	private FamilyPool families = new FamilyPool();

	private Dictionary<int, FamiliesInvolved> familiesInvolvedDict = new Dictionary<int, FamiliesInvolved>();

	public PersonPool People => people;
	public FamilyPool Families => families;
	
	#region Helper State Functions

	public bool IsPeopleEmpty() => people?.Pool?.Count == 0;
	public bool IsFamiliesEmpty() => families?.Pool?.Count == 0;
	public bool IsFamiliesInvolvedEmpty() => familiesInvolvedDict?.Count == 0;
	public bool IsEmpty() => IsPeopleEmpty() && IsFamiliesEmpty() && IsFamiliesInvolvedEmpty();
	
	public PersonID ContainsPerson(Person person)
	{
		for (int i = 0; i < people.Pool.Count; i++)
			if (people.Pool[i].IsEqual(person))
				return new PersonID(i);

		return new PersonID(-1);
	}
	
	public FamilyID ContainsFamily(Family family)
	{
		for (int i = 0; i < families.Pool.Count; i++)
			if (families.Pool[i].IsEqual(family))
				return new FamilyID(i);

		return new FamilyID(-1);
	}

	public Person LookupPerson(PersonID person) => people.Lookup(person);

	public Family LookupFamily(FamilyID family) => families.Lookup(family);

	#endregion

	private void AddFamiliesInvolvedAsChild(PersonID person, FamilyID family)
	{
		if (!familiesInvolvedDict.ContainsKey(person.Value))
			familiesInvolvedDict[person.Value] = new FamiliesInvolved();
		
		familiesInvolvedDict[person.Value].ChildOf = family;
	}
	
	private void AddFamiliesInvolvedAsOwner(PersonID person, FamilyID family)
	{
		if (!familiesInvolvedDict.ContainsKey(person.Value))
			familiesInvolvedDict[person.Value] = new FamiliesInvolved();

		familiesInvolvedDict[person.Value].OwnerOf = family;
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
		if (!familiesInvolvedDict.ContainsKey(parent.Value) || !familiesInvolvedDict.ContainsKey(child.Value)) return false;
		if (parent.Equals(child)) return true;

		var parentFamily = families.Lookup(familiesInvolvedDict[child.Value].ChildOf);
		if (parentFamily == null) return false;
		
		return CheckParentRelation(parent, parentFamily.Father) || CheckParentRelation(parent, parentFamily.Mother);
	}

	public bool CheckSiblingRelation(PersonID first, PersonID second) // if 'first' is a sibling of 'second'
	{
		if (!people.ValidateID(first) || !people.ValidateID(second)) return false;
		if (!familiesInvolvedDict.ContainsKey(first.Value) || !familiesInvolvedDict.ContainsKey(second.Value)) return false;
		
		var firstFamily = familiesInvolvedDict[first.Value].ChildOf;
		var secondFamily = familiesInvolvedDict[second.Value].ChildOf;

		if (firstFamily.Value == -1 || secondFamily.Value == -1) return false;

		return firstFamily.Equals(secondFamily);
	}

	public bool CheckDistantRelation(PersonID first, PersonID second)
	{
		if (!people.ValidateID(first) || !people.ValidateID(second)) return false;
		
		var firstFamily = familiesInvolvedDict[first.Value].ChildOf;
		var secondFamily = familiesInvolvedDict[second.Value].ChildOf;
		
		return firstFamily.Equals(secondFamily);
	}
}
