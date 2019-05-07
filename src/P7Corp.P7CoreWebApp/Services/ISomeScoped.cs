namespace P7Corp.P7CoreWebApp.Services
{
    public interface ISomeLazyScoped : ISomeScoped { }
    public interface ISomeScoped
    {
        string Name { get; }
    }
}