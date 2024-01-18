using System;
using UnityEngine;
using System.Diagnostics;
using Unity.Burst;

public class PerformanceTester : MonoBehaviour
{
    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();

        stopwatch.Start();
        BFSTest();
        stopwatch.Stop();
        
        TimeSpan timeTaken = stopwatch.Elapsed;
        UnityEngine.Debug.Log("BFS took: " + timeTaken.ToString(@"m\:ss\.fff"));

        stopwatch.Reset();
        stopwatch.Start();
        FastTest();
        stopwatch.Stop();
        
        timeTaken = stopwatch.Elapsed;
        UnityEngine.Debug.Log("Fast took: " + timeTaken.ToString(@"m\:ss\.fff"));
    }
    
    void BFSTest()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();

        Person father = new Person("Father");
        PersonID fatherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);

        Person childMother = new Person("ChildMother");
        PersonID childMotherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(childf);

        Person childChild = new Person("ChildChild");
        PersonID childChildid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        Person childFatherBrother = new Person("ChildFatherBrother");
        PersonID childFatherBrotherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrother);
        ft.AddChildToFamily(childFatherBrotherid, fid);
        
        Person childFatherBrotherWife = new Person("ChildFatherBrotherWife");
        PersonID childFatherBrotherWifeid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherWife);
        
        Family childFatherBrotherf = new Family(childFatherBrotherid, childFatherBrotherWifeid);
        FamilyID childFatherBrotherfid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(childFatherBrotherf);
        
        Person childFatherBrotherSon = new Person("ChildFatherBrotherSon");
        PersonID childFatherBrotherSonid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherSon);
        ft.AddChildToFamily(childFatherBrotherSonid, childFatherBrotherfid);
        
        Person childFatherBrotherSon2 = new Person("ChildFatherBrotherSon2");
        PersonID childFatherBrotherSon2id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherSon2);
        ft.AddChildToFamily(childFatherBrotherSon2id, childFatherBrotherfid);
        
        Person person = new Person("Person");
        PersonID personid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(person);

        //Act
        bool isChildChildADistantRelativeOfChildFatherBrother = false;
        bool isChildFatherBrotherSonADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = false;
        bool isChildChildADistantRelativeOfPerson = true;
        
        for (int i = 0; i < 200000; i++)
        {
            isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationBFS(childChildid, childFatherBrotherid); // True
            isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSonid, childChildid); // True
            isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childChildid); // True
            isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationBFS(childFatherBrotherSon2id, childFatherBrotherSonid); //True
            isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationBFS(childChildid, personid); //False
        }
    }
    
    void FastTest()
    {
        //Arrange
        FamilyTree ft = new FamilyTree();

        Person father = new Person("Father");
        PersonID fatherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(father);
        Person mother = new Person("Mother");
        PersonID motherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(mother);

        Family f = new Family(fatherid, motherid);
        FamilyID fid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(f);

        Person childFather = new Person("ChildFather");
        PersonID childFatherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFather);
        ft.AddChildToFamily(childFatherid, fid);

        Person childMother = new Person("ChildMother");
        PersonID childMotherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childMother);

        Family childf = new Family(childFatherid, childMotherid);
        FamilyID childfid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(childf);

        Person childChild = new Person("ChildChild");
        PersonID childChildid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childChild);
        ft.AddChildToFamily(childChildid, childfid);
        
        Person childFatherBrother = new Person("ChildFatherBrother");
        PersonID childFatherBrotherid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrother);
        ft.AddChildToFamily(childFatherBrotherid, fid);
        
        Person childFatherBrotherWife = new Person("ChildFatherBrotherWife");
        PersonID childFatherBrotherWifeid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherWife);
        
        Family childFatherBrotherf = new Family(childFatherBrotherid, childFatherBrotherWifeid);
        FamilyID childFatherBrotherfid = (FamilyID)ft.Families.GenerateUniqueID();
        ft.AddFamily(childFatherBrotherf);
        
        Person childFatherBrotherSon = new Person("ChildFatherBrotherSon");
        PersonID childFatherBrotherSonid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherSon);
        ft.AddChildToFamily(childFatherBrotherSonid, childFatherBrotherfid);
        
        Person childFatherBrotherSon2 = new Person("ChildFatherBrotherSon2");
        PersonID childFatherBrotherSon2id = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(childFatherBrotherSon2);
        ft.AddChildToFamily(childFatherBrotherSon2id, childFatherBrotherfid);
        
        Person person = new Person("Person");
        PersonID personid = (PersonID)ft.People.GenerateUniqueID();
        ft.AddPerson(person);

        //Act
        bool isChildChildADistantRelativeOfChildFatherBrother = false;
        bool isChildFatherBrotherSonADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildChild = false;
        bool isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = false;
        bool isChildChildADistantRelativeOfPerson = true;
        
        for (int i = 0; i < 200000; i++)
        {
            isChildChildADistantRelativeOfChildFatherBrother = ft.CheckDistantRelationFast(childChildid, childFatherBrotherid); // True
            isChildFatherBrotherSonADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSonid, childChildid); // True
            isChildFatherBrotherSon2ADistantRelativeOfChildChild = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childChildid); // True
            isChildFatherBrotherSon2ADistantRelativeOfChildFatherBrotherSon = ft.CheckDistantRelationFast(childFatherBrotherSon2id, childFatherBrotherSonid); //True
            isChildChildADistantRelativeOfPerson = ft.CheckDistantRelationFast(childChildid, personid); //False
        }
    }
}
