using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using P7Core.Writers;
using Shouldly;
using Xunit;

namespace XUnitTestProject_P7CorpP7Core
{
    public class JsonDocumentWriterUnitTest
    {
        [Fact]
        public async Task Test_Utils_SerializeObject_string()
        {
            string original = "dog";
            string expected = $"\"{original}\"";
            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(true);
            jsonDocumentWriter.JsonSerializerSettings.ShouldNotBeNull();
            var actual = jsonDocumentWriter.SerializeObject(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_SerializeObjectSingleQuote_string()
        {
            string original = "dog";
            string expected = $"'{original}'";
            JsonDocumentWriter jsonDocumentWriter= new JsonDocumentWriter(true);
            var actual = jsonDocumentWriter.SerializeObjectSingleQuote(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_SerializeObjectSingleQuote_object()
        {
            object original = new 
            {
                value = "dog"
            };
            string inside = $"{Environment.NewLine}  'value': 'dog'{Environment.NewLine}";
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append(inside);
            sb.Append('}');

            string expected = sb.ToString();
           
            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(true);
            var actual = jsonDocumentWriter.SerializeObjectSingleQuote(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_SerializeObject_object()
        {
            object original = new
            {
                value = "dog"
            };
            string inside = $"{Environment.NewLine}  \"value\": \"dog\"{Environment.NewLine}";
            StringBuilder sb = new StringBuilder();
            sb.Append('{');
            sb.Append(inside);
            sb.Append('}');

            string expected = sb.ToString();

            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(true);
            var actual = jsonDocumentWriter.SerializeObject(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_SerializeObjectSingleQuote_array_string()
        {
            object original = new[]
            {
                "dog"
            };
            string inside = $"{Environment.NewLine}  'dog'{Environment.NewLine}";
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(inside);
            sb.Append(']');
            string expected = sb.ToString();

            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(true);
            var actual = jsonDocumentWriter.SerializeObjectSingleQuote(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_SerializeObject_array_string()
        {
            object original = new[]
            {
                "dog"
            };
            string inside = $"{Environment.NewLine}  \"dog\"{Environment.NewLine}";
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            sb.Append(inside);
            sb.Append(']');
            string expected = sb.ToString();

            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(true);
            var actual = jsonDocumentWriter.SerializeObject(original);
            expected.ShouldBe(actual);
        }
        class JsonDocumentWriterOptions : IJsonDocumentWriterOptions
        {

            public Formatting Formatting => Formatting.Indented;

            public JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                DateTimeZoneHandling = DateTimeZoneHandling.Utc,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter>()
                {
                    new Newtonsoft.Json.Converters.IsoDateTimeConverter()
                }
            };
        }
        [Fact]
        public async Task Test_Utils_JsonDocumentWriterOptions_SerializeObject_string()
        {
            JsonDocumentWriterOptions options = new JsonDocumentWriterOptions();
            string original = "dog";
            string expected = $"\"{original}\"";
            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter(options);
            var actual = jsonDocumentWriter.SerializeObject(original);
            expected.ShouldBe(actual);
        }
        [Fact]
        public async Task Test_Utils_default_SerializeObject_string()
        {
           
            string original = "dog";
            string expected = $"\"{original}\"";
            JsonDocumentWriter jsonDocumentWriter = new JsonDocumentWriter();
            var actual = jsonDocumentWriter.SerializeObject(original);
            expected.ShouldBe(actual);
        }
    }
}
