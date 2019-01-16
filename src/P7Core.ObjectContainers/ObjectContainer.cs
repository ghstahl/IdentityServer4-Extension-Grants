namespace P7Core.ObjectContainers
{
    public class ObjectContainer<TContaining, TObject> :
        ISingletonObjectContainer<TContaining, TObject>,
        IScopedObjectContainer<TContaining, TObject>
        where TContaining : class
        where TObject : class

    {
        private IObjectAllocator<TContaining, TObject> _allocator;
        public ObjectContainer() { }

        public ObjectContainer(IObjectAllocator<TContaining, TObject> allocator)
        {
            _allocator = allocator;
        }

        private TObject _value;

        public TObject Value
        {
            get
            {
                if (_value == null && _allocator != null)
                {
                    _value = _allocator.Allocate();
                }

                return _value;
            }
            set { _value = value; }
        }
    }
}