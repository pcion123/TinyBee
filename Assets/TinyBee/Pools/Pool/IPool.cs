namespace TinyBee.Pool
{
    public interface IPool<T>
    {
        T Allocate();

        bool Recycle(T obj);
    }
}