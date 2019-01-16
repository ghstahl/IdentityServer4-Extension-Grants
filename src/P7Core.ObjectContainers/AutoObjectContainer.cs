namespace P7Core.ObjectContainers
{
    public class AutoObjectContainer<TContaining, TObject> :
        ISingletonAutoObjectContainer<TContaining, TObject>,
        IScopedAutoObjectContainer<TContaining, TObject>
        where TContaining : class
        where TObject : class

    {
        private IAutoObjectAllocator<TContaining, TObject> _allocator;


        public AutoObjectContainer(IAutoObjectAllocator<TContaining, TObject> allocator)
        {
            _allocator = allocator;
        }

        private TObject _value;

        public TObject Value
        {
            get
            {
                if (_value == null)
                {
                    _value = _allocator.Allocate();
                }

                return _value;
            }
            set { _value = value; }
        }
    }
}