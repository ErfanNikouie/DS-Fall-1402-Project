using NUnit.Framework;

public class MD5HashTests
{
    [Test]
    public void MD5Hash_Simple_Test()
    {
        //Arrange
        string s = "Hello World!";
        string resultSA = "";
        string resultSB = "";
        
        //Act

        resultSA = MD5Hash.CalculateHash(s);
        resultSB = MD5Hash.CalculateHash(s);
        
        //Assert
        Assert.AreEqual(resultSA, resultSB);
    }
}
