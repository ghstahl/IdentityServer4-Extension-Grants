namespace P7Corp.P7CoreWebApp.Services
{
    public interface ISomeLazyTransient : ISomeTransient { }
    public interface ISomeTransient
    {
        string Name { get; }
    }
}