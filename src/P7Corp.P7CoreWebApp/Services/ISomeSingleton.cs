namespace P7Corp.P7CoreWebApp.Services
{
    public interface ISomeLazySingleton : ISomeSingleton { }
    public interface ISomeSingleton
    {
        string Name { get; }
    }
}