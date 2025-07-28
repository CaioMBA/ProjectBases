using Dapper;
using Domain.Extensions;
using System.Data;

namespace Data.Database.DapperHandlers
{
    public class DictionaryTypeHandler : SqlMapper.TypeHandler<IDictionary<string, object?>>
    {
        public override void SetValue(IDbDataParameter parameter, IDictionary<string, object?>? value)
        {
            parameter.Value = value?.ToJson();
        }

        public override IDictionary<string, object?>? Parse(object? value)
        {
            try
            {
                if (value == null)
                {
                    return null;
                }
                if (value is string json && !String.IsNullOrEmpty(json))
                {
                    return json.ToObject<Dictionary<string, object?>>()
                            ?? throw new DataException("JSON is not an object.");
                }
                throw new DataException("Unexpected data type when parsing JSON.");
            }
            catch (Exception ex)
            {
                return new Dictionary<string, object?>() {
                            { "response", value } ,
                            { "parsingError", ex.Message },
                            { "parsingStackTrace", ex.StackTrace },
                            { "parsingType", value?.GetType().Name }
                };
            }
        }
    }
}
