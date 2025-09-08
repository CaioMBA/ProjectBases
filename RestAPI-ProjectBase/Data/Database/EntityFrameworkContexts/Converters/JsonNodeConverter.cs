using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class JsonNodeConverter : ValueConverter<JsonNode, string>
    {
        public JsonNodeConverter() : base(
            v => v.ToJsonString(new JsonSerializerOptions()),

            v => JsonNode.Parse(v, null, default)!
        )
        { }
    }

    public class NullableJsonNodeConverter : ValueConverter<JsonNode?, string?>
    {
        public NullableJsonNodeConverter() : base(
            v => v == null ? null : v.ToJsonString(new JsonSerializerOptions()),

            v => string.IsNullOrEmpty(v) ? null : JsonNode.Parse(v, null, default)
        )
        { }
    }
}
