namespace P7Corp.P7CoreWebApp.Services
{
    public class SomeTransient : ISomeTransient, ISomeLazyTransient
    {
        public string Name => nameof(SomeTransient);
    }
}