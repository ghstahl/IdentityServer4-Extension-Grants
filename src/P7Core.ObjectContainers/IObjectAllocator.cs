namespace P7Core.ObjectContainers
{
    public interface IObjectAllocator<TContaining, out T> where TContaining : class where T : class
    {
        T Allocate();
    }
}