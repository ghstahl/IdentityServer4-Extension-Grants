namespace P7Corp.P7CoreWebApp.Services
{
    public class SomeSingleton : ISomeSingleton, ISomeLazySingleton
    {
        public string Name => nameof(SomeSingleton);
    }
}