namespace P7Core.ObjectContainers
{
    public class ObjectAllocator<TContaining, TObject> :
        IObjectAllocator<TContaining, TObject>,
        IAutoObjectAllocator<TContaining, TObject>
        where TContaining : class
        where TObject : class, new()
    {
        public TObject Allocate()
        {
            return new TObject();
        }
    }
}