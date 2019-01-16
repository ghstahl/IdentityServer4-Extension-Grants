namespace P7Core.ObjectContainers
{
    public interface IAutoObjectAllocator<TContaining, out T> where TContaining : class where T : class
    {
        T Allocate();
    }
}