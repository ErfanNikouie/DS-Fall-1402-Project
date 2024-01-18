using NUnit.Framework;

public class FamilyTreeTests
{
    [Test]
    public void FamilyTreeTestsSimplePasses()
    {
        //Arrange
        //Act
        //Assert
    }

    [Test]
    public void FamilyTree_Constructor_Test()
    {
        //Arrange & Act
        FamilyTree ft = new FamilyTree();
        
        //Assert
        Assert.AreEqual(true, ft.IsPeopleEmpty());
        Assert.AreEqual(true, ft.IsFamiliesEmpty());
        Assert.AreEqual(true, ft.IsFamiliesInvolvedEmpty());
        Assert.AreEqual(true, ft.IsEmpty());
    }
    
    [Test]
    public void FamilyTree_AddPerson_Single_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        Person p = new Person("SomeName");
        PersonID pid = (PersonID) ft.People.GenerateUniqueID();
        
        //Act
        ft.AddPerson(p);
        
        //Assert
        Assert.AreEqual(1, ft.People.Pool.Count);
        
        Assert.AreEqual(0, pid.Value);
        Assert.AreEqual(0, ft.ContainsPerson(p).Value);
        Assert.AreEqual("SomeName", ft.LookupPerson(pid).Name);
    }
    
    [Test]
    public void FamilyTree_AddPerson_Multiple_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        Person p0 = new Person("SomeName0");
        PersonID p0id = (PersonID) ft.People.GenerateUniqueID();
        
        //Act
        ft.AddPerson(p0);
        
        Person p1 = new Person("SomeName1");
        PersonID p1id = (PersonID) ft.People.GenerateUniqueID();
        
        //Act
        ft.AddPerson(p1);
        
        //Assert
        Assert.AreEqual(2, ft.People.Pool.Count);
        
        Assert.AreEqual(0, p0id.Value);
        Assert.AreEqual(0, ft.ContainsPerson(p0).Value);
        Assert.AreEqual("SomeName0", ft.LookupPerson(p0id).Name);
        
        Assert.AreEqual(1, p1id.Value);
        Assert.AreEqual(1, ft.ContainsPerson(p1).Value);
        Assert.AreEqual("SomeName1", ft.LookupPerson(p1id).Name);
    }
    
    [Test]
    public void FamilyTree_AddFamily_Single_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        
        Person father = new Person("Father");
        PersonID fatherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.Families.GenerateUniqueID();
        
        //Act
        ft.AddFamily(f);
        
        //Assert
        Assert.AreEqual(1, ft.Families.Pool.Count);
        
        Assert.AreEqual(0, fid.Value);
        Assert.AreEqual(0, ft.ContainsFamily(f).Value);
        
        Assert.AreEqual(0, ft.LookupFamily(fid).Father.Value);
        Assert.AreEqual(1, ft.LookupFamily(fid).Mother.Value);
    }
    
    [Test]
    public void FamilyTree_AddChildToFamily_Multiple_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        
        Person father = new Person("Father");
        PersonID fatherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person child0 = new Person("Child0");
        PersonID child0id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child0);
        
        ft.AddChildToFamily(child0id, fid);
        
        Person child1 = new Person("Child1");
        PersonID child1id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child1);
        
        ft.AddChildToFamily(child1id, fid);
        
        //Assert
        Assert.AreEqual(1, ft.Families.Pool.Count);
        Assert.AreEqual(2, ft.LookupFamily(fid).Children.Count);
        
        Assert.AreEqual(0, ft.LookupFamily(fid).Father.Value);
        Assert.AreEqual(1, ft.LookupFamily(fid).Mother.Value);
        
        Assert.AreEqual(2, ft.LookupFamily(fid).Children[0].Value);
        Assert.AreEqual(3, ft.LookupFamily(fid).Children[1].Value);
    }

    [Test]
    public void FamilyTree_CheckParentRelation_SingleLayer_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        
        Person father = new Person("Father");
        PersonID fatherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person child0 = new Person("Child0");
        PersonID child0id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child0);
        ft.AddChildToFamily(child0id, fid);
        
        Person child1 = new Person("Child1");
        PersonID child1id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child1);
        ft.AddChildToFamily(child1id, fid);
        
        Person person = new Person("Person");
        PersonID personid = (PersonID) ft.People.GenerateUniqueID();
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
    public void FamilyTree_CheckParentRelation_MultiLayer_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        
        Person father = new Person("Father");
        PersonID fatherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);
        
        Person childMother = new Person("ChildMother");
        PersonID childMotherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = (FamilyID) ft.Families.GenerateUniqueID();
        ft.AddFamily(childf);
        
        Person childChild = new Person("ChildChild");
        PersonID childChildid = (PersonID) ft.People.GenerateUniqueID();
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
    public void FamilyTree_CheckSiblingRelation_Test()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();
        
        Person father = new Person("Father");
        PersonID fatherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID) ft.People.GenerateUniqueID();
        ft.AddPerson(mother);
        
        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID) ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person child0 = new Person("Child0");
        PersonID child0id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child0);
        ft.AddChildToFamily(child0id, fid);
        
        Person child1 = new Person("Child1");
        PersonID child1id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(child1);
        ft.AddChildToFamily(child1id, fid);
        
        Person person = new Person("Person");
        PersonID personid = (PersonID) ft.People.GenerateUniqueID();
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
}
