using System;
using System.Collections.Generic;

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
	

	private void BFSBackwardsTrace(PersonID first, Action<FamilyID> onTraced) //Child to each parent
	{
		Queue<FamilyID> trace = new Queue<FamilyID>();

		trace.Enqueue(familiesInvolvedDict[first.Value].ChildOf);

		FamilyID fid = new FamilyID(-1);
		FamilyID fatherFamily = new FamilyID(-1);
		FamilyID motherFamily = new FamilyID(-1);
		
		while (trace.Count > 0)
		{
			fid = trace.Dequeue();
			onTraced?.Invoke(fid);

			fatherFamily = familiesInvolvedDict[families.Lookup(fid).Father.Value].ChildOf;
			motherFamily = familiesInvolvedDict[families.Lookup(fid).Mother.Value].ChildOf;
			
			if(fatherFamily.Value != -1)
				trace.Enqueue(fatherFamily);
			if(motherFamily.Value != -1)
				trace.Enqueue(motherFamily);
		}
	}
	

	private void FastDoubleBackwardsTrace(PersonID first, PersonID second, Action<FamilyID> onMet)
	{
		Queue<FamilyID> firstTrace = new Queue<FamilyID>();
		List<FamilyID> firstMarked = new List<FamilyID>();
		Queue<FamilyID> secondTrace = new Queue<FamilyID>();
		List<FamilyID> secondMarked = new List<FamilyID>();
		
		firstTrace.Enqueue(familiesInvolvedDict[first.Value].ChildOf);
		secondTrace.Enqueue(familiesInvolvedDict[second.Value].ChildOf);
		
		FamilyID fid = new FamilyID(-1);
		FamilyID fatherFamily = new FamilyID(-1);
		FamilyID motherFamily = new FamilyID(-1);

		while (firstTrace.Count != 0 || secondTrace.Count != 0)
		{
			if (firstTrace.Count > 0)
			{
				fid = firstTrace.Dequeue();
				firstMarked.Add(fid);

				fatherFamily = familiesInvolvedDict[families.Lookup(fid).Father.Value].ChildOf;
				motherFamily = familiesInvolvedDict[families.Lookup(fid).Mother.Value].ChildOf;
				
				if(fatherFamily.Value != -1)
					firstTrace.Enqueue(fatherFamily);
				if(motherFamily.Value != -1)
					firstTrace.Enqueue(motherFamily);
			}

			if (secondTrace.Count > 0)
			{
				fid = secondTrace.Dequeue();
				secondMarked.Add(fid);

				fatherFamily = familiesInvolvedDict[families.Lookup(fid).Father.Value].ChildOf;
				motherFamily = familiesInvolvedDict[families.Lookup(fid).Mother.Value].ChildOf;

				if (fatherFamily.Value != -1)
					secondTrace.Enqueue(fatherFamily);
				if (motherFamily.Value != -1)
					secondTrace.Enqueue(motherFamily);
			}
			

			foreach (FamilyID fmark in firstMarked)
				foreach (FamilyID smark in secondMarked)
					if (fmark.Equals(smark))
					{
						onMet?.Invoke(fmark);
						return;
					}
		}
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
	

	public bool CheckDistantRelationBFS(PersonID first, PersonID second) => GetCommonFamiliesBFS(first, second)?.Count > 0;

	public bool CheckDistantRelationFast(PersonID first, PersonID second) => GetCommonFamilyFast(first, second).Value != -1;
	
	public List<FamilyID> GetCommonFamiliesBFS(PersonID first, PersonID second)
	{
		if (!people.ValidateID(first) || !people.ValidateID(second)) return null;
		if (!familiesInvolvedDict.ContainsKey(first.Value) || !familiesInvolvedDict.ContainsKey(second.Value)) return null;
		if (familiesInvolvedDict[first.Value].ChildOf.Value == -1 || familiesInvolvedDict[second.Value].ChildOf.Value == -1) return null;

		List<FamilyID> commonFamilies = new List<FamilyID>();
		List<FamilyID> firstFamilies = new List<FamilyID>();

		BFSBackwardsTrace(first, id => firstFamilies.Add(id)); //Mark first parent families
		BFSBackwardsTrace(second, //Filter out the common families
			id => 
			{ 
				foreach (FamilyID f in firstFamilies)
				{
					if (f.Value != id.Value) continue;
					
					commonFamilies.Add(id);
					firstFamilies.Remove(f);
					break;
				}
			});

		return commonFamilies;
	}
	
	public FamilyID GetCommonFamilyFast(PersonID first, PersonID second)
	{
		FamilyID result = new FamilyID(-1);
		
		if (!people.ValidateID(first) || !people.ValidateID(second)) return result;
		if (!familiesInvolvedDict.ContainsKey(first.Value) || !familiesInvolvedDict.ContainsKey(second.Value)) return result;
		if (familiesInvolvedDict[first.Value].ChildOf.Value == -1 || familiesInvolvedDict[second.Value].ChildOf.Value == -1) return result;

		
		FastDoubleBackwardsTrace(first, second, id => result = id);
		return result;
	}
}
