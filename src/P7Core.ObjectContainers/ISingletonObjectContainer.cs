namespace P7Core.ObjectContainers
{
    public interface ISingletonObjectContainer<TContaining, T> : IObjectContainer<TContaining, T>
        where TContaining : class
        where T : class
    {
    }
}