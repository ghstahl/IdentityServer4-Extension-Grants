namespace P7Core.ObjectContainers
{
    public interface IScopedObjectContainer<TContaining, T> : IObjectContainer<TContaining, T>
        where TContaining : class
        where T : class
    {
    }
}