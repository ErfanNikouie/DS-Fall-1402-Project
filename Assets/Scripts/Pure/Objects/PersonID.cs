using System;

[Serializable]
public struct PersonID : IID
{
	private int value;
	public int Value => value;

	public PersonID(int value = -1) => this.value = value;
}
