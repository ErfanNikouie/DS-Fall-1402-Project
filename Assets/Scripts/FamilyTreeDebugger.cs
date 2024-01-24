using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class FamilyTreeDebugger : MonoBehaviour
{
    void Start()
    {
        Test();
    }

    void Test()
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
        
        //Act
        ft.GetFarthestRelation(fatherid, out PersonID ffirst, out PersonID fsecond, out int fdist);
        ft.GetFarthestRelation(motherid, out PersonID mfirst, out PersonID msecond, out int mdist);
    }
}
