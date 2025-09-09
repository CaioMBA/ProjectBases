using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Data.Database.EntityFrameworkContexts.Comparers
{
    public static class JsonValueComparers
    {
        private static readonly JsonSerializerOptions JsonOpts = new();

        // ---- JsonObject
        public static readonly ValueComparer<JsonObject?> JsonObjectNullableComparer =
            new(
                (l, r) => JsonNode.DeepEquals(l, r),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
                v => v == null ? null : (JsonObject)v.DeepClone()
            );

        // ---- JsonArray
        public static readonly ValueComparer<JsonArray?> JsonArrayNullableComparer =
            new(
                (l, r) => JsonNode.DeepEquals(l, r),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
                v => v == null ? null : (JsonArray)v.DeepClone()
            );

        // ---- JsonNode (base)
        public static readonly ValueComparer<JsonNode?> JsonNodeNullableComparer =
            new(
                (l, r) => JsonNode.DeepEquals(l, r),
                v => v == null ? 0 : JsonSerializer.Serialize(v, JsonOpts).GetHashCode(),
                v => v == null ? null : v.DeepClone()
            );
    }
}
