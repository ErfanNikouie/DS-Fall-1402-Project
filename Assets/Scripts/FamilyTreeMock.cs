using UnityEngine;

public class FamilyTreeMock : MonoBehaviour
{
    [SerializeField] private FamilyTreeView view;

    public void CreateFamily(FamilyTreeController ft)
    {
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
    }
}
