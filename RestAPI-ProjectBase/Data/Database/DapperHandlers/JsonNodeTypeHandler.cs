using Dapper;
using System.Data;
using System.Text.Json.Nodes;

namespace Data.Database.DapperHandlers
{
    public class JsonNodeTypeHandler : SqlMapper.TypeHandler<JsonNode>
    {
        public override void SetValue(IDbDataParameter parameter, JsonNode? value)
        {
            parameter.Value = value?.ToJsonString();
        }

        public override JsonNode? Parse(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string json)
            {
                return JsonNode.Parse(json) ?? throw new DataException("JSON could not be parsed.");
            }

            throw new DataException("Unexpected data type when parsing JSON.");
        }
    }
}
