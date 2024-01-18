public interface IID
{
    public int Value { get; }

    public bool Equals(IID id) => Value == id.Value;
}
