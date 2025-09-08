using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class JsonArrayConverter : ValueConverter<JsonArray, string>
    {
        public JsonArrayConverter() : base(
            v => v.ToJsonString(new JsonSerializerOptions()),

            v => JsonNode.Parse(v, null, default)!.AsArray()
        )
        { }
    }

    public class NullableJsonArrayConverter : ValueConverter<JsonArray?, string?>
    {
        public NullableJsonArrayConverter() : base(
            v => v == null ? null : v.ToJsonString(new JsonSerializerOptions()),

            v => string.IsNullOrEmpty(v) ? null : JsonNode.Parse(v, null, default)!.AsArray()
        )
        { }
    }
}
