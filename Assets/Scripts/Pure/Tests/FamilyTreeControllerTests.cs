using NUnit.Framework;

public class FamilyTreeControllerTests
{
	[Test]
	public void FamilyTreeController_Constructor_Test()
	{
		//Arrange & Act
		FamilyTreeController ft = new FamilyTreeController();
        
		//Assert
		Assert.AreEqual(true, ft.IsPeopleEmpty());
		Assert.AreEqual(true, ft.IsFamiliesEmpty());
		Assert.AreEqual(true, ft.IsFamiliesInvolvedEmpty());
		Assert.AreEqual(true, ft.IsEmpty());
	}
	
	[Test]
	public void FamilyTreeController_AddPerson_Single_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
		Person p = new Person("SomeName");
		PersonID pid = ft.GetUniquePersonID();
        
		//Act
		ft.AddPerson(p);
        
		//Assert
		Assert.AreEqual(1, ft.PeopleCount);
        
		Assert.AreEqual(0, pid.Value);
		Assert.AreEqual(0, ft.ContainsPerson(p).Value);
		Assert.AreEqual(MD5Hash.CalculateHash("SomeName"), ft.LookupPerson(pid).Name);
	}
	
	[Test]
	public void FamilyTreeController_AddPerson_Multiple_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
		Person p0 = new Person("SomeName0");
		PersonID p0id = ft.GetUniquePersonID();
        
		//Act
		ft.AddPerson(p0);
        
		Person p1 = new Person("SomeName1");
		PersonID p1id = ft.GetUniquePersonID();
        
		//Act
		ft.AddPerson(p1);
        
		//Assert
		Assert.AreEqual(2, ft.PeopleCount);
        
		Assert.AreEqual(0, p0id.Value);
		Assert.AreEqual(0, ft.ContainsPerson(p0).Value);
		Assert.AreEqual(MD5Hash.CalculateHash("SomeName0"), ft.LookupPerson(p0id).Name);
        
		Assert.AreEqual(1, p1id.Value);
		Assert.AreEqual(1, ft.ContainsPerson(p1).Value);
		Assert.AreEqual(MD5Hash.CalculateHash("SomeName1"), ft.LookupPerson(p1id).Name);
	}
	
	[Test]
	public void FamilyTreeController_AddFamily_Single_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
        
		Person father = new Person("Father");
		PersonID fatherid = ft.GetUniquePersonID();
		ft.AddPerson(father);
		Person mother = new Person("Mother");
		PersonID motherid = ft.GetUniquePersonID();
		ft.AddPerson(mother);

		Family f = new Family(fatherid, motherid);
		FamilyID fid = ft.GetUniqueFamilyID();
        
		//Act
		ft.AddFamily(f);
        
		//Assert
		Assert.AreEqual(1, ft.FamilyCount);
        
		Assert.AreEqual(0, fid.Value);
		Assert.AreEqual(0, ft.ContainsFamily(f).Value);
        
		Assert.AreEqual(0, ft.LookupFamily(fid).Father.Value);
		Assert.AreEqual(1, ft.LookupFamily(fid).Mother.Value);
	}
	
	[Test]
	public void FamilyTreeController_AddChildToFamily_Multiple_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
        
		Person father = new Person("Father");
		PersonID fatherid = ft.GetUniquePersonID();
		ft.AddPerson(father);
		Person mother = new Person("Mother");
		PersonID motherid = ft.GetUniquePersonID();
		ft.AddPerson(mother);
        
		Family f = new Family(fatherid, motherid);
		FamilyID fid = ft.GetUniqueFamilyID();
		ft.AddFamily(f);

		Person child0 = new Person("Child0");
		PersonID child0id = ft.GetUniquePersonID();
		ft.AddPerson(child0);
        
		ft.AddChildToFamily(child0id, fid);
        
		Person child1 = new Person("Child1");
		PersonID child1id = ft.GetUniquePersonID();
		ft.AddPerson(child1);
        
		ft.AddChildToFamily(child1id, fid);
        
		//Assert
		Assert.AreEqual(1, ft.FamilyCount);
		Assert.AreEqual(2, ft.LookupFamily(fid).Children.Count);
        
		Assert.AreEqual(0, ft.LookupFamily(fid).Father.Value);
		Assert.AreEqual(1, ft.LookupFamily(fid).Mother.Value);
        
		Assert.AreEqual(2, ft.LookupFamily(fid).Children[0].Value);
		Assert.AreEqual(3, ft.LookupFamily(fid).Children[1].Value);
	}
	
	[Test]
	public void FamilyTreeController_CheckParentRelation_SingleLayer_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
        
		Person father = new Person("Father");
		PersonID fatherid = ft.GetUniquePersonID();
		ft.AddPerson(father);
		Person mother = new Person("Mother");
		PersonID motherid = ft.GetUniquePersonID();
		ft.AddPerson(mother);
        
		Family f = new Family(fatherid, motherid);
		FamilyID fid = ft.GetUniqueFamilyID();
		ft.AddFamily(f);

		Person child0 = new Person("Child0");
		PersonID child0id = ft.GetUniquePersonID();
		ft.AddPerson(child0);
		ft.AddChildToFamily(child0id, fid);
        
		Person child1 = new Person("Child1");
		PersonID child1id = ft.GetUniquePersonID();
		ft.AddPerson(child1);
		ft.AddChildToFamily(child1id, fid);
        
		Person person = new Person("Person");
		PersonID personid = ft.GetUniquePersonID();
		ft.AddPerson(person);
        
		//Act
		bool isChild0AChildOfFather = ft.CheckParentRelation(fatherid, child0id); //True
		bool isChild1AChildOfFather = ft.CheckParentRelation(fatherid, child1id); //True
		bool isPersonAChildOfFather = ft.CheckParentRelation(fatherid, personid); //False
		bool isFatherAChildOfChild0 = ft.CheckParentRelation(child0id, fatherid); //False
        
        
		//Assert
		Assert.IsTrue(isChild0AChildOfFather);
		Assert.IsTrue(isChild1AChildOfFather);
		Assert.IsFalse(isPersonAChildOfFather);
		Assert.IsFalse(isFatherAChildOfChild0);
	}
	
	[Test]
    public void FamilyTreeController_CheckParentRelation_MultiLayer_Test()
    {
        //Arrange
        FamilyTreeController ft = new FamilyTreeController();
        
        Person father = new Person("Father");
        PersonID fatherid = ft.GetUniquePersonID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = ft.GetUniquePersonID();
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = ft.GetUniqueFamilyID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = ft.GetUniquePersonID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);
        
        Person childMother = new Person("ChildMother");
        PersonID childMotherid = ft.GetUniquePersonID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childf);
        
        Person childChild = new Person("ChildChild");
        PersonID childChildid = ft.GetUniquePersonID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        //Act
        bool isChildChildAChildOfFather = ft.CheckParentRelation(fatherid, childChildid); //True
        bool isChildChildAChildOfChildFather = ft.CheckParentRelation(childFatherid, childChildid); //True
        bool isChildMotherAChildOfFather = ft.CheckParentRelation(fatherid, childMotherid); //False
        bool isChildMotherAChildOfChildFather = ft.CheckParentRelation(childFatherid, childMotherid); //False
        
        //Assert
        Assert.IsTrue(isChildChildAChildOfFather);
        Assert.IsTrue(isChildChildAChildOfChildFather);
        Assert.IsFalse(isChildMotherAChildOfFather);
        Assert.IsFalse(isChildMotherAChildOfChildFather);
    }
    
	[Test]
	public void FamilyTreeController_CheckSiblingRelation_Test()
	{
		//Arrange
		FamilyTreeController ft = new FamilyTreeController();
        
		Person father = new Person("Father");
		PersonID fatherid = ft.GetUniquePersonID();
		ft.AddPerson(father);
		Person mother = new Person("Mother");
		PersonID motherid = ft.GetUniquePersonID();
		ft.AddPerson(mother);
        
		Family f = new Family(fatherid, motherid);
		FamilyID fid = ft.GetUniqueFamilyID();
		ft.AddFamily(f);

		Person child0 = new Person("Child0");
		PersonID child0id = ft.GetUniquePersonID();
		ft.AddPerson(child0);
		ft.AddChildToFamily(child0id, fid);
        
		Person child1 = new Person("Child1");
		PersonID child1id = ft.GetUniquePersonID();
		ft.AddPerson(child1);
		ft.AddChildToFamily(child1id, fid);
        
		Person person = new Person("Person");
		PersonID personid = ft.GetUniquePersonID();
		ft.AddPerson(person);
        
		//Act
		bool isChild0ASiblingOfChild1 = ft.CheckSiblingRelation(child0id, child1id); //True
		bool isChild1ASiblingOfChild0 = ft.CheckSiblingRelation(child1id, child0id); //True
		bool isFatherASiblingOfMother = ft.CheckSiblingRelation(fatherid, motherid); //False
		bool isChild0ASiblingOfMother = ft.CheckSiblingRelation(child0id, motherid); //False
		bool isPersonASiblingOfChild0 = ft.CheckSiblingRelation(personid, child0id); //False
        
		//Assert
		Assert.IsTrue(isChild0ASiblingOfChild1);
		Assert.IsTrue(isChild1ASiblingOfChild0);
		Assert.IsFalse(isFatherASiblingOfMother);
		Assert.IsFalse(isChild0ASiblingOfMother);
		Assert.IsFalse(isPersonASiblingOfChild0);
	}
	
	[Test]
    public void FamilyTreeController_CheckDistantRelationBFS_Single_Test()
    {
        //Arrange
        FamilyTreeController ft = new FamilyTreeController();

        Person father = new Person("Father");
        PersonID fatherid = ft.GetUniquePersonID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = ft.GetUniquePersonID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = ft.GetUniqueFamilyID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = ft.GetUniquePersonID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);

        Person childMother = new Person("ChildMother");
        PersonID childMotherid = ft.GetUniquePersonID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childf);

        Person childChild = new Person("ChildChild");
        PersonID childChildid = ft.GetUniquePersonID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        Person childFatherBrother = new Person("ChildFatherBrother");
        PersonID childFatherBrotherid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrother);
        ft.AddChildToFamily(childFatherBrotherid, fid);
        
        Person childFatherBrotherWife = new Person("ChildFatherBrotherWife");
        PersonID childFatherBrotherWifeid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherWife);
        
        Family childFatherBrotherf = new Family(childFatherBrotherid, childFatherBrotherWifeid);
        FamilyID childFatherBrotherfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childFatherBrotherf);
        
        Person childFatherBrotherSon = new Person("ChildFatherBrotherSon");
        PersonID childFatherBrotherSonid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon);
        ft.AddChildToFamily(childFatherBrotherSonid, childFatherBrotherfid);
        
        Person childFatherBrotherSon2 = new Person("ChildFatherBrotherSon2");
        PersonID childFatherBrotherSon2id = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon2);
        ft.AddChildToFamily(childFatherBrotherSon2id, childFatherBrotherfid);
        
        Person person = new Person("Person");
        PersonID personid = ft.GetUniquePersonID();
        ft.AddPerson(person);

        //Act
        bool isChildChildADistantRelativeOfChildFatherBrother = false;
        bool isChildFatherBrotherSonADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = false;
        bool isChildChildADistantRelativeOfPerson = true;
        
//        for (int i = 0; i < 200000; i++)
//        {
//            isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationBFS(childChildid, childFatherBrotherid); // True
//            isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSonid, childChildid); // True
//            isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childChildid); // True
//            isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childFatherBrotherSonid); //True
//            isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationBFS(childChildid, personid); //False
//        }
        
        isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationBFS(childChildid, childFatherBrotherid); // True
        isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSonid, childChildid); // True
        isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childChildid); // True
        isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childFatherBrotherSonid); //True
        isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationBFS(childChildid, personid); //False
        
        

        //Assert
        Assert.IsTrue(isChildChildADistantRelativeOfChildFatherBrother);
        Assert.IsTrue(isChildFatherBrotherSonADistantRelativeOfChildChild);
        Assert.IsTrue(isChildFatherBrotherSon2ADistantRelativeOfChildChild);
        Assert.IsTrue(isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon);
        Assert.IsFalse(isChildChildADistantRelativeOfPerson);
    }
    
    [Test]
    public void FamilyTreeController_CheckDistantRelationFast_Single_Test()
    {
        //Arrange
        FamilyTreeController ft = new FamilyTreeController();

        Person father = new Person("Father");
        PersonID fatherid = ft.GetUniquePersonID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = ft.GetUniquePersonID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = ft.GetUniqueFamilyID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = ft.GetUniquePersonID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);

        Person childMother = new Person("ChildMother");
        PersonID childMotherid = ft.GetUniquePersonID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childf);

        Person childChild = new Person("ChildChild");
        PersonID childChildid = ft.GetUniquePersonID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        Person childFatherBrother = new Person("ChildFatherBrother");
        PersonID childFatherBrotherid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrother);
        ft.AddChildToFamily(childFatherBrotherid, fid);
        
        Person childFatherBrotherWife = new Person("ChildFatherBrotherWife");
        PersonID childFatherBrotherWifeid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherWife);
        
        Family childFatherBrotherf = new Family(childFatherBrotherid, childFatherBrotherWifeid);
        FamilyID childFatherBrotherfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childFatherBrotherf);
        
        Person childFatherBrotherSon = new Person("ChildFatherBrotherSon");
        PersonID childFatherBrotherSonid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon);
        ft.AddChildToFamily(childFatherBrotherSonid, childFatherBrotherfid);
        
        Person childFatherBrotherSon2 = new Person("ChildFatherBrotherSon2");
        PersonID childFatherBrotherSon2id = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon2);
        ft.AddChildToFamily(childFatherBrotherSon2id, childFatherBrotherfid);
        
        Person person = new Person("Person");
        PersonID personid = ft.GetUniquePersonID();
        ft.AddPerson(person);

        //Act
        bool isChildChildADistantRelativeOfChildFatherBrother = false;
        bool isChildFatherBrotherSonADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = false;
        bool isChildChildADistantRelativeOfPerson = true;
        
//        for (int i = 0; i < 200000; i++)
//        {
//            isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationFast(childChildid, childFatherBrotherid); // True
//            isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSonid, childChildid); // True
//            isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childChildid); // True
//            isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childFatherBrotherSonid); //True
//            isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationFast(childChildid, personid); //False
//        }
        
        isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationFast(childChildid, childFatherBrotherid); // True
        isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSonid, childChildid); // True
        isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childChildid); // True
        isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childFatherBrotherSonid); //True
        isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationFast(childChildid, personid); //False
        

        //Assert
        Assert.IsTrue(isChildChildADistantRelativeOfChildFatherBrother);
        Assert.IsTrue(isChildFatherBrotherSonADistantRelativeOfChildChild);
        Assert.IsTrue(isChildFatherBrotherSon2ADistantRelativeOfChildChild);
        Assert.IsTrue(isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon);
        Assert.IsFalse(isChildChildADistantRelativeOfPerson);
    }
    
    [Test]
    public void FamilyTreeController_GetFarthestBorn_MultiLayer_Test()
    {
        //Arrange
        FamilyTreeController ft = new FamilyTreeController();
        
        Person father = new Person("Father");
        PersonID fatherid = ft.GetUniquePersonID(); //fatherid: 0
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = ft.GetUniquePersonID(); //fatherid: 1
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.GetUniqueFamilyID(); //familyid: 0
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = ft.GetUniquePersonID(); //childFatherid: 2
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);
        
        Person childMother = new Person("ChildMother");
        PersonID childMotherid = ft.GetUniquePersonID(); //childMotherid: 3
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = ft.GetUniqueFamilyID(); //childfid: 1
        ft.AddFamily(childf);
        
        Person childChild = new Person("ChildChild");
        PersonID childChildid = ft.GetUniquePersonID(); //childChildid: 4
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        //Act
        PersonID farthestBornOfFather = ft.GetFarthestBorn(fatherid, out int fatherdist);
        PersonID farthestBornOfMother = ft.GetFarthestBorn(motherid, out int motherdist);
        PersonID farthestBornOfChildFather = ft.GetFarthestBorn(childFatherid, out int childfdist);
        PersonID farthestBornOfChildMother = ft.GetFarthestBorn(childMotherid, out int childmdist);
        PersonID farthestBornOfChildChild = ft.GetFarthestBorn(childChildid, out int childcdist);
        
        //Assert
        Assert.AreEqual(4, farthestBornOfFather.Value);
        Assert.AreEqual(4, farthestBornOfMother.Value);
        Assert.AreEqual(4, farthestBornOfChildFather.Value);
        Assert.AreEqual(4, farthestBornOfChildMother.Value);
        Assert.AreEqual(4, farthestBornOfChildChild.Value);
        
        Assert.AreEqual(2, fatherdist);
        Assert.AreEqual(2, motherdist);
        Assert.AreEqual(1, childfdist);
        Assert.AreEqual(1, childmdist);
        Assert.AreEqual(0, childcdist);
    }
    
    [Test]
    public void FamilyTreeController_GetFarthestRelation_MultiLayer_Test()
    {
        //Arrange
        FamilyTreeController ft = new FamilyTreeController();

        Person father = new Person("Father");
        PersonID fatherid = ft.GetUniquePersonID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = ft.GetUniquePersonID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = ft.GetUniqueFamilyID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = ft.GetUniquePersonID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);

        Person childMother = new Person("ChildMother");
        PersonID childMotherid = ft.GetUniquePersonID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childf);

        Person childChild = new Person("ChildChild");
        PersonID childChildid = ft.GetUniquePersonID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        Person childFatherBrother = new Person("ChildFatherBrother");
        PersonID childFatherBrotherid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrother);
        ft.AddChildToFamily(childFatherBrotherid, fid);
        
        Person childFatherBrotherWife = new Person("ChildFatherBrotherWife");
        PersonID childFatherBrotherWifeid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherWife);
        
        Family childFatherBrotherf = new Family(childFatherBrotherid, childFatherBrotherWifeid);
        FamilyID childFatherBrotherfid = ft.GetUniqueFamilyID();
        ft.AddFamily(childFatherBrotherf);
        
        Person childFatherBrotherSon = new Person("ChildFatherBrotherSon");
        PersonID childFatherBrotherSonid = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon);
        ft.AddChildToFamily(childFatherBrotherSonid, childFatherBrotherfid);
        
        Person childFatherBrotherSon2 = new Person("ChildFatherBrotherSon2");
        PersonID childFatherBrotherSon2id = ft.GetUniquePersonID();
        ft.AddPerson(childFatherBrotherSon2);
        ft.AddChildToFamily(childFatherBrotherSon2id, childFatherBrotherfid);
        
        //Act
        ft.GetFarthestRelation(fatherid, out PersonID ffirst, out PersonID fsecond, out int fdist);
        ft.GetFarthestRelation(motherid, out PersonID mfirst, out PersonID msecond, out int mdist);
        
        //Assert
        Assert.AreEqual(4, ffirst.Value);
        Assert.AreEqual(7, fsecond.Value);
        Assert.AreEqual(4, mfirst.Value);
        Assert.AreEqual(7, msecond.Value);
        
        Assert.AreEqual(3, fdist);
        Assert.AreEqual(3, mdist);
    }
}
