using System;

[Serializable]
public class Person
{
    private string name;
    public string Name => name;

    public Person(string name) => this.name = MD5Hash.CalculateHash(name);

    public bool IsEqual(Person person) => person.Name == this.name;
}
