using System;

[Serializable]
public class Person
{
    private string name;
    public string Name => name;

    public Person(string name) => this.name = name;

    public bool IsEqual(Person person) => person.Name == this.name;
}
