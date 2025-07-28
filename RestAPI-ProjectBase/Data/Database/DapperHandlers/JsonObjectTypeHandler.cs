using Dapper;
using System.Data;
using System.Text.Json.Nodes;

namespace Data.Database.DapperHandlers
{
    public class JsonObjectTypeHandler : SqlMapper.TypeHandler<JsonObject>
    {
        public override void SetValue(IDbDataParameter parameter, JsonObject? value)
        {
            parameter.Value = value?.ToJsonString();
        }

        public override JsonObject? Parse(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string json)
            {
                var node = JsonNode.Parse(json);
                if (node is JsonObject obj)
                    return obj;

                throw new DataException("JSON is not an object.");
            }

            throw new DataException("Unexpected data type when parsing JSON.");
        }
    }
}
