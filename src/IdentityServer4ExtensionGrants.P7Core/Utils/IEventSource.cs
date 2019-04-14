namespace P7Core.Utils
{
    public interface IEventSource<T>
    {
        void RegisterEventSink(T sink);
        void UnregisterEventSink(T sink);
        void UnregisterAll();
    }
}