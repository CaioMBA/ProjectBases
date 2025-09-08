using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Data.Database.EntityFrameworkContexts.Converters
{
    public class JsonObjectConverter : ValueConverter<JsonObject, string>
    {
        public JsonObjectConverter() : base(
            v => v.ToJsonString(new JsonSerializerOptions()),

            v => JsonNode.Parse(v, null, default)!.AsObject()
        )
        { }
    }

    public class NullableJsonObjectConverter : ValueConverter<JsonObject?, string?>
    {
        public NullableJsonObjectConverter() : base(
            v => v == null ? null : v.ToJsonString(new JsonSerializerOptions()),
            v => string.IsNullOrEmpty(v) ? null : JsonNode.Parse(v, null, default)!.AsObject()
        )
        { }
    }
}
