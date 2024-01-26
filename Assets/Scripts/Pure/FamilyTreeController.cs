using System;
using System.Collections.Generic;

public class FamilyTreeController
{
    private FamilyTreeModel model = new FamilyTreeModel();
    
    #region Model State Functions
    
    // quick census
    public int PeopleCount => model.PeopleCount;
    public int FamilyCount => model.FamilyCount;

    public PersonPool People => model.People;
    public FamilyPool Families => model.Families;
    
    public bool IsPeopleEmpty() => model.IsPeopleEmpty();
    public bool IsFamiliesEmpty() => model.IsFamiliesEmpty();
    public bool IsFamiliesInvolvedEmpty() => model.IsFamiliesInvolvedEmpty();
    public bool IsEmpty() => model.IsEmpty();
    
    public PersonID GetUniquePersonID() => model.GetUniquePersonID();
    public FamilyID GetUniqueFamilyID() => model.GetUniqueFamilyID();

    public PersonID ContainsPerson(Person person) => model.ContainsPerson(person);

    public FamilyID ContainsFamily(Family family) => model.ContainsFamily(family);
    
    public Person LookupPerson(PersonID person) => model.LookupPerson(person);

    public Family LookupFamily(FamilyID family) => model.LookupFamily(family);
    
    public bool TryGetFamiliesInvolved(PersonID person, out FamiliesInvolved result) => model.TryGetFamiliesInvolved(person, out result);
    public bool TryGetFamiliesInvolved(int person, out FamiliesInvolved result) => model.TryGetFamiliesInvolved(person, out result);
    
    #endregion

    #region Events

    public void RegisterAddPersonListener(Action<int> onPersonAdded) => model.OnPersonAdded += onPersonAdded;
    public void RegisterAddFamilyListener(Action<int> onFamilyAdded) => model.OnFamilyAdded += onFamilyAdded;
    public void RegisterAddChildListener(Action<int, int> onChildAdded) => model.OnChildAdded += onChildAdded;
    
    #endregion
    
    #region Internal
    
    private void BFSBackwardsTrace(PersonID first, Action<FamilyID> onTraced) //Child to each parent
    {
        Queue<FamilyID> trace = new Queue<FamilyID>();
        
        model.TryGetFamiliesInvolved(first, out var firstFamilies);
        trace.Enqueue(firstFamilies.ChildOf);

        FamilyID fid = new FamilyID(-1);
        FamilyID fatherFamily = new FamilyID(-1);
        FamilyID motherFamily = new FamilyID(-1);
		
        while (trace.Count > 0)
        {
            fid = trace.Dequeue();
            onTraced?.Invoke(fid);
            
            model.TryGetFamiliesInvolved(model.LookupFamily(fid).Father, out var fatherFamilies);
            model.TryGetFamiliesInvolved(model.LookupFamily(fid).Mother, out var motherFamilies);

            fatherFamily = fatherFamilies.ChildOf;
            motherFamily = motherFamilies.ChildOf;
			
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


		model.TryGetFamiliesInvolved(first, out var firstFamilies);
		model.TryGetFamiliesInvolved(second, out var secondFamilies);
		firstTrace.Enqueue(firstFamilies.ChildOf);
		secondTrace.Enqueue(secondFamilies.ChildOf);
		
		FamilyID fid = new FamilyID(-1);
		FamilyID fatherFamily = new FamilyID(-1);
		FamilyID motherFamily = new FamilyID(-1);

		while (firstTrace.Count != 0 || secondTrace.Count != 0)
		{
			if (firstTrace.Count > 0)
			{
				fid = firstTrace.Dequeue();
				firstMarked.Add(fid);

				model.TryGetFamiliesInvolved(model.LookupFamily(fid).Father, out var fatherFamilies);
				model.TryGetFamiliesInvolved(model.LookupFamily(fid).Mother, out var motherFamilies);
				fatherFamily = fatherFamilies.ChildOf;
				motherFamily = motherFamilies.ChildOf;
				
				if(fatherFamily.Value != -1)
					firstTrace.Enqueue(fatherFamily);
				if(motherFamily.Value != -1)
					firstTrace.Enqueue(motherFamily);
			}

			if (secondTrace.Count > 0)
			{
				fid = secondTrace.Dequeue();
				secondMarked.Add(fid);

				model.TryGetFamiliesInvolved(model.LookupFamily(fid).Father, out var fatherFamilies);
				model.TryGetFamiliesInvolved(model.LookupFamily(fid).Mother, out var motherFamilies);
				fatherFamily = fatherFamilies.ChildOf;
				motherFamily = motherFamilies.ChildOf;

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

			model.TryGetFamiliesInvolved(model.LookupFamily(fid).Father, out var fatherFamilies);
			model.TryGetFamiliesInvolved(model.LookupFamily(fid).Mother, out var motherFamilies);
			fatherFamily = fatherFamilies.ChildOf;
			motherFamily = motherFamilies.ChildOf;

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

			model.TryGetFamiliesInvolved(model.LookupFamily(fid).Father, out var fatherFamilies);
			model.TryGetFamiliesInvolved(model.LookupFamily(fid).Mother, out var motherFamilies);
			fatherFamily = fatherFamilies.ChildOf;
			motherFamily = motherFamilies.ChildOf;

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
		Family f = model.LookupFamily(fid);
			
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
    
    #endregion
    
    public bool AddPerson(Person person) => model.AddPerson(person);
    public bool AddFamily(Family family) => model.AddFamily(family);
    public bool AddChildToFamily(PersonID child, FamilyID family) => model.AddChildToFamily(child, family);
    
    public bool CheckParentRelation(PersonID parent, PersonID child) // if 'parent' is a parent of 'child'
    {
        if (!model.ValidatePersonID(parent) || !model.ValidatePersonID(child)) return false;
        if (!model.TryGetFamiliesInvolved(parent, out var parentFamilies) || !model.TryGetFamiliesInvolved(child, out var childFamilies)) return false;
        if (parent.Equals(child)) return true;

        var parentFamily = model.LookupFamily(childFamilies.ChildOf);
        if (parentFamily == null) return false;
		
        return CheckParentRelation(parent, parentFamily.Father) || CheckParentRelation(parent, parentFamily.Mother);
    }
    
    public bool CheckSiblingRelation(PersonID first, PersonID second) // if 'first' is a sibling of 'second'
    {
        if (!model.ValidatePersonID(first) || !model.ValidatePersonID(second)) return false;
        if (!model.TryGetFamiliesInvolved(first, out var firstFamilies) || !model.TryGetFamiliesInvolved(second, out var secondFamilies)) return false;
		
        var firstFamily = firstFamilies.ChildOf;
        var secondFamily = secondFamilies.ChildOf;

        if (firstFamily.Value == -1 || secondFamily.Value == -1) return false;

        return firstFamily.Equals(secondFamily);
    }
    
    public List<FamilyID> GetCommonFamiliesBFS(PersonID first, PersonID second)
    {
        if (!model.ValidatePersonID(first) || !model.ValidatePersonID(second)) return null;
        if (!model.TryGetFamiliesInvolved(first, out var fFamilies) || !model.TryGetFamiliesInvolved(second, out var sFamilies)) return null;
        if (fFamilies.ChildOf.Value == -1 || sFamilies.ChildOf.Value == -1) return null;

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
		
        if (!model.ValidatePersonID(first) || !model.ValidatePersonID(second)) return result;
        if (!model.TryGetFamiliesInvolved(first, out var fFamilies) || !model.TryGetFamiliesInvolved(second, out var sFamilies)) return result;
        if (fFamilies.ChildOf.Value == -1 || sFamilies.ChildOf.Value == -1) return result;
		
        FastDoubleBackwardsTrace(first, second, id => result = id);
        return result;
    }
    
    public bool CheckDistantRelationBFS(PersonID first, PersonID second) => GetCommonFamiliesBFS(first, second)?.Count > 0;
    public bool CheckDistantRelationFast(PersonID first, PersonID second) => GetCommonFamilyFast(first, second).Value != -1;
    
    public PersonID GetFarthestBorn(PersonID person, out int distance)
    {
	    distance = 0;

	    if (!model.ValidatePersonID(person) || !model.TryGetFamiliesInvolved(person, out var personFamily)) return new PersonID(-1);

	    Family family = model.LookupFamily(personFamily.OwnerOf);
		
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
	    if (!model.ValidatePersonID(root) || !model.TryGetFamiliesInvolved(root, out var rootFamily)) return;
		
	    first = GetFarthestBorn(root, out int firstDist);
		
	    Queue<PersonID> trace = new Queue<PersonID>();

	    trace.Enqueue(first);
	    Dictionary<int, int> distanceToNodes = new Dictionary<int, int> { { first.Value, 0 } };
	    HashSet<int> visitedNodes = new HashSet<int>();
		
	    while (trace.Count > 0)
	    {
		    PersonID pid = trace.Dequeue();
		    if(pid.Value == -1) continue;

		    model.TryGetFamiliesInvolved(pid, out var pFamily);
			
		    //if(!visitedNodes.Add(pid.Value)) continue;
		    int currentDistance = distanceToNodes[pid.Value];
		    AddFamilyToTrace(pid, pFamily.ChildOf, currentDistance, ref second, ref distance, ref trace, ref distanceToNodes, ref visitedNodes,
			    (PersonID id, ref PersonID personID, ref int i) =>
			    {
				    if (distanceToNodes[id.Value] <= i) return;
					
				    i = distanceToNodes[id.Value];
				    personID = id;
			    });
			
		    AddFamilyToTrace(pid, pFamily.OwnerOf, currentDistance, ref second, ref distance, ref trace, ref distanceToNodes, ref visitedNodes,
			    (PersonID id, ref PersonID personID, ref int i) =>
			    {
				    if (distanceToNodes[id.Value] <= i) return;
					
				    i = distanceToNodes[id.Value];
				    personID = id;
			    });
	    }
    }
}
