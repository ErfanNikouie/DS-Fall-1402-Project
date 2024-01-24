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
		}
		
		foreach (FamilyID fmark in firstMarked)
			foreach (FamilyID smark in secondMarked)
				if (fmark.Equals(smark))
				{
					onMet?.Invoke(fmark);
					return;
				}
		
		while (secondTrace.Count > 0)
		{
			fid = secondTrace.Dequeue();
			foreach (FamilyID f in firstMarked)
			{
				if (f.Value != fid.Value) continue;
				onMet?.Invoke(fid);
				return;
			}

			fatherFamily = familiesInvolvedDict[families.Lookup(fid).Father.Value].ChildOf;
			motherFamily = familiesInvolvedDict[families.Lookup(fid).Mother.Value].ChildOf;

			if (fatherFamily.Value != -1)
				secondTrace.Enqueue(fatherFamily);
			if (motherFamily.Value != -1)
				secondTrace.Enqueue(motherFamily);
		}
		
		while (firstTrace.Count > 0)
		{
			fid = firstTrace.Dequeue();
			foreach (FamilyID f in secondMarked)
			{
				if (f.Value != fid.Value) continue;
				onMet?.Invoke(fid);
				return;
			}

			fatherFamily = familiesInvolvedDict[families.Lookup(fid).Father.Value].ChildOf;
			motherFamily = familiesInvolvedDict[families.Lookup(fid).Mother.Value].ChildOf;

			if (fatherFamily.Value != -1)
				secondTrace.Enqueue(fatherFamily);
			if (motherFamily.Value != -1)
				secondTrace.Enqueue(motherFamily);
		}
	}
	
	delegate void TraceCallback<T, U, V>(T first, ref U second, ref V distance);
	private void AddFamilyToTrace(PersonID pid, FamilyID fid, int currentDistance,
		ref PersonID second, ref int distance,
		ref Queue<PersonID> trace, ref Dictionary<int, int> distanceToNodes,
		ref HashSet<int> visitedNodes, TraceCallback<PersonID, PersonID, int> onTraced)
	{
		if(fid.Value == -1) return;
		Family f = families.Lookup(fid);
			
		if (!visitedNodes.Contains(f.Father.Value))
		{
			trace.Enqueue(f.Father);
			distanceToNodes[f.Father.Value] = currentDistance + 1;
			visitedNodes.Add(f.Father.Value);
		}
		
		if (!visitedNodes.Contains(f.Mother.Value))
		{
			trace.Enqueue(f.Mother);
			distanceToNodes[f.Mother.Value] = currentDistance + 1;
			visitedNodes.Add(f.Mother.Value);
		}
			
		foreach (var child in f.Children)
		{
			if(!visitedNodes.Add(child.Value) || child.Equals(pid)) continue;
				
			trace.Enqueue(child);
			distanceToNodes[child.Value] = currentDistance + 1;
			onTraced?.Invoke(child, ref second, ref distance);
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

	public PersonID GetFarthestBorn(PersonID person, out int distance)
	{
		distance = 0;

		if (!people.ValidateID(person) || !familiesInvolvedDict.TryGetValue(person.Value, out var personFamily)) return new PersonID(-1);

		Family family = families.Lookup(personFamily.OwnerOf);
		
		if (family == null || family.Children.Count == 0) return person;
		
		distance++;

		int count = family.Children.Count;
		
		int[] childrenDistance = new int[count];
		PersonID[] childrenResult = new PersonID[count];

		for (int i = 0; i < count; i++)
			childrenResult[i] = GetFarthestBorn(family.Children[i], out childrenDistance[i]);

		int maxDistance = -1;
		PersonID result = person;
		for (int i = 0; i < count; i++)
		{
			if (childrenDistance[i] <= maxDistance)
				continue;

			maxDistance = childrenDistance[i];
			result = childrenResult[i];
		}

		distance += maxDistance;
		return result;
	}

	public void GetFarthestRelation(PersonID root, out PersonID first, out PersonID second, out int distance)
	{
		first = new PersonID(-1);
		second = new PersonID(-1);
		distance = 0;
		if (!people.ValidateID(root) || !familiesInvolvedDict.TryGetValue(root.Value, out var rootFamily)) return;
		
		first = GetFarthestBorn(root, out int firstDist);
		
		Queue<PersonID> trace = new Queue<PersonID>();

		trace.Enqueue(first);
		Dictionary<int, int> distanceToNodes = new Dictionary<int, int> { { first.Value, 0 } };
		HashSet<int> visitedNodes = new HashSet<int>();
		
		while (trace.Count > 0)
		{
			PersonID pid = trace.Dequeue();
			if(pid.Value == -1) continue;
			
			//if(!visitedNodes.Add(pid.Value)) continue;
			int currentDistance = distanceToNodes[pid.Value];
			AddFamilyToTrace(pid, familiesInvolvedDict[pid.Value].ChildOf, currentDistance, ref second, ref distance, ref trace, ref distanceToNodes, ref visitedNodes,
				(PersonID id, ref PersonID personID, ref int i) =>
				{
					if (distanceToNodes[id.Value] <= i) return;
					
					i = distanceToNodes[id.Value];
					personID = id;
				});
			
			AddFamilyToTrace(pid, familiesInvolvedDict[pid.Value].OwnerOf, currentDistance, ref second, ref distance, ref trace, ref distanceToNodes, ref visitedNodes,
				(PersonID id, ref PersonID personID, ref int i) =>
				{
					if (distanceToNodes[id.Value] <= i) return;
					
					i = distanceToNodes[id.Value];
					personID = id;
				});

			/*FamilyID fcid = familiesInvolvedDict[pid.Value].ChildOf;
			if(fcid.Value == -1) continue;
			
			int currentDistance = distanceToNodes[pid.Value] + 1;
			Family fc = families.Lookup(fcid);

			if (!visitedNodes.Contains(fc.Father.Value))
			{
				trace.Enqueue(fc.Father);
				distanceToNodes[fc.Father.Value] = currentDistance;
			}
			
			if (!visitedNodes.Contains(fc.Mother.Value))
			{
				trace.Enqueue(fc.Mother);
				distanceToNodes[fc.Mother.Value] = currentDistance;
			}
			
			foreach (var child in fc.Children)
			{
				if(visitedNodes.Contains(child.Value) || child.Equals(pid)) continue;
				
				trace.Enqueue(child);
				distanceToNodes[child.Value] = currentDistance;
				
				// Update the farthest person and distance
				if (distanceToNodes[child.Value] <= distance) continue;
				
				distance = distanceToNodes[child.Value];
				second = child;
			}
			
			FamilyID foid = familiesInvolvedDict[pid.Value].OwnerOf;
			if(foid.Value == -1) continue;
			Family fo = families.Lookup(foid);
			
			if (!visitedNodes.Contains(fo.Father.Value))
			{
				trace.Enqueue(fo.Father);
				distanceToNodes[fo.Father.Value] = currentDistance;
			}
			
			if (!visitedNodes.Contains(fo.Mother.Value))
			{
				trace.Enqueue(fo.Mother);
				distanceToNodes[fo.Mother.Value] = currentDistance;
			}
			
			foreach (var child in fo.Children)
			{
				if(visitedNodes.Contains(child.Value) || child.Equals(pid)) continue;
				
				trace.Enqueue(child);
				distanceToNodes[child.Value] = currentDistance;
				
				// Update the farthest person and distance
				if (distanceToNodes[child.Value] <= distance) continue;
				
				distance = distanceToNodes[child.Value];
				second = child;
			}*/
		}
	}
}
