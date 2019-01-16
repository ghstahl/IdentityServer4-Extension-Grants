namespace P7Core.ObjectContainers
{
    public interface IObjectContainer<TContaining, T> where TContaining : class where T : class
    {
        T Value { get; set; }
    }
}