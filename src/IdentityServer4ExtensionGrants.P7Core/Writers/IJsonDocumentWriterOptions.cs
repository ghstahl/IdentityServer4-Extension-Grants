using Newtonsoft.Json;

namespace P7Core.Writers
{
    public interface IJsonDocumentWriterOptions
    {
        Formatting Formatting { get; }
        JsonSerializerSettings JsonSerializerSettings { get; }
    }
}