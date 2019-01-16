namespace P7Core.ObjectContainers
{
    public interface ISingletonAutoObjectContainer<TContaining, T> : IObjectContainer<TContaining, T>
        where TContaining : class
        where T : class
    {
    }
}