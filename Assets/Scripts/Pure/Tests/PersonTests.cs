using NUnit.Framework;

public class PersonTests
{
	[Test]
	public void Person_Constructor_WithSomeName_Test()
	{
		//Arrange
		Person p = new Person("SomeName");
		
		//Assert
		Assert.AreEqual(MD5Hash.CalculateHash("SomeName"), p.Name);
	}
}
