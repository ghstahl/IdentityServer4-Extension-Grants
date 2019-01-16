namespace P7Core.ObjectContainers
{
    public interface IScopedAutoObjectContainer<TContaining, T> : IObjectContainer<TContaining, T>
        where TContaining : class
        where T : class
    {
    }
}
