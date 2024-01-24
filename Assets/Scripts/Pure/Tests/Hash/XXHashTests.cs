using NUnit.Framework;

public class XXHashTests
{
	[Test]
	public void XXHash_Simple_Test()
	{
		//Arrange
		string s = "Hello World!";
		string resultSA = "";
		string resultSB = "";
		string resultSC = "";
		string resultSD = "";
        
		//Act

		resultSA = XXHash.CalculateHash(s).ToString();
		resultSB = XXHash.CalculateHash(s).ToString();
		resultSC = XXHash.CalculateHash(s).ToString();
		resultSD = XXHash.CalculateHash(s).ToString();

        
		//Assert
		Assert.AreEqual(resultSA, resultSB);
		Assert.AreEqual(resultSC, resultSD);
		Assert.AreEqual(resultSB, resultSC);
	}
}