using NUnit.Framework;

public class FamilyIDTests
{
    [Test]
    public void FamilyID_Constructor_WithDefault0ID_Test()
    {
        //Arrange
        //Act
        FamilyID id = new FamilyID();

        //Assert
        Assert.AreEqual(0, id.Value);
    }
    
    [Test]
    public void FamilyID_Constructor_With0ID_Test()
    {
        //Arrange
        //Act
        FamilyID id = new FamilyID(0);

        //Assert
        Assert.AreEqual(0, id.Value);
    }
    
    [Test]
    public void FamilyID_Constructor_With1ID_Test()
    {
        //Arrange
        //Act
        FamilyID id = new FamilyID(1);

        //Assert
        Assert.AreEqual(1, id.Value);
    }
    
    [Test]
    public void FamilyID_Constructor_WithNegative1ID_Test()
    {
        //Arrange
        //Act
        FamilyID id = new FamilyID(-1);
        
        //Assert
        Assert.AreEqual(-1, id.Value);
    }
}
