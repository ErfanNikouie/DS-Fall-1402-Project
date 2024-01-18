using NUnit.Framework;

public class FamilyTests
{
    [Test]
    public void Family_Constructor_WithDefaultIDs_Test()
    {
        //Arrange
        PersonID father = new PersonID();
        PersonID mother = new PersonID();
        
        //Act
        Family f = new Family(father, mother);
        
        //Assert
        Assert.AreEqual(0, f.Father.Value);
        Assert.AreEqual(0, f.Mother.Value);
    }
    
    [Test]
    public void Family_Constructor_With1IDs_Test()
    {
        //Arrange
        PersonID father = new PersonID(1);
        PersonID mother = new PersonID(1);
        
        //Act
        Family f = new Family(father, mother);
        
        //Assert
        Assert.AreEqual(1, f.Father.Value);
        Assert.AreEqual(1, f.Mother.Value);
    }
    
    [Test]
    public void Family_Constructor_WithNegative1IDs_Test()
    {
        //Arrange
        PersonID father = new PersonID(-1);
        PersonID mother = new PersonID(-1);
        
        //Act
        Family f = new Family(father, mother);
        
        //Assert
        Assert.AreEqual(-1, f.Father.Value);
        Assert.AreEqual(-1, f.Mother.Value);
    }
    
    [Test]
    public void Family_AddChild_WithSomeRepeatingIDs_Test()
    {
        //Arrange
        PersonID father = new PersonID();
        PersonID mother = new PersonID();
        
        PersonID child0 = new PersonID(-1);
        PersonID child1 = new PersonID(0);
        PersonID child2 = new PersonID();
        PersonID child3 = new PersonID(1);
        PersonID child4 = new PersonID(1);
        
        Family f = new Family(father, mother);
        
        //Act
        f.AddChild(child0);
        f.AddChild(child1);
        f.AddChild(child2);
        f.AddChild(child3);
        f.AddChild(child4);
        
        //Assert
        Assert.AreEqual(3, f.Children.Count);
        Assert.AreEqual(-1, f.Children[0].Value);
        Assert.AreEqual(0, f.Children[1].Value);
        Assert.AreEqual(1, f.Children[2].Value);
    }
    
    [Test]
    public void Family_AddChild_WithSomeUniqueIDs_Test()
    {
        //Arrange
        PersonID father = new PersonID();
        PersonID mother = new PersonID();
        
        PersonID child0 = new PersonID(-1);
        PersonID child1 = new PersonID(0);
        PersonID child2 = new PersonID(1);
        PersonID child3 = new PersonID(2);
        
        Family f = new Family(father, mother);
        
        //Act
        f.AddChild(child0);
        f.AddChild(child1);
        f.AddChild(child2);
        f.AddChild(child3);
        
        //Assert
        Assert.AreEqual(4, f.Children.Count);
        Assert.AreEqual(-1, f.Children[0].Value);
        Assert.AreEqual(0, f.Children[1].Value);
        Assert.AreEqual(1, f.Children[2].Value);
        Assert.AreEqual(2, f.Children[3].Value);
    }
}
