namespace TinyBee.Pool
{
    public interface IObjectFactory<T>
    {
        T Create();
    }
}