using Dapper;
using System.Data;
using System.Text.Json.Nodes;

namespace Data.Database.DapperHandlers
{
    public class JsonArrayTypeHandler : SqlMapper.TypeHandler<JsonArray>
    {
        public override void SetValue(IDbDataParameter parameter, JsonArray? value)
        {
            parameter.Value = value?.ToJsonString();
        }

        public override JsonArray Parse(object value)
        {
            if (value is string json)
            {
                var node = JsonNode.Parse(json);
                if (node is JsonArray arr)
                    return arr;

                throw new DataException("JSON is not an array.");
            }

            throw new DataException("Unexpected data type when parsing JSON.");
        }
    }
}
