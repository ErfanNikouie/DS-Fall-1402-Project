using NUnit.Framework;

public class PersonIDTests
{
    [Test]
    public void PersonID_Constructor_WithDefault0ID_Test()
    {
        //Arrange
        //Act
        PersonID id = new PersonID();

        //Assert
        Assert.AreEqual(0, id.Value);
    }
    
    [Test]
    public void PersonID_Constructor_With0ID_Test()
    {
        //Arrange
        //Act
        PersonID id = new PersonID(0);

        //Assert
        Assert.AreEqual(0, id.Value);
    }
    
    [Test]
    public void PersonID_Constructor_With1ID_Test()
    {
        //Arrange
        //Act
        PersonID id = new PersonID(1);

        //Assert
        Assert.AreEqual(1, id.Value);
    }
    
    [Test]
    public void PersonID_Constructor_WithNegative1ID_Test()
    {
        //Arrange
        //Act
        PersonID id = new PersonID(-1);
        
        //Assert
        Assert.AreEqual(-1, id.Value);
    }
}
