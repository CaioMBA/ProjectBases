using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domain.Extensions
{
    public static class ObjectExtension
    {
        public static string ToJson(this Object obj)
        {
            byte[] jsonBytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, new JsonSerializerOptions
            {
                WriteIndented = false,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            string jsonResponse = Encoding.UTF8.GetString(jsonBytes);

            if (String.IsNullOrEmpty(jsonResponse) || jsonResponse == "{}")
            {
                jsonResponse = JsonConvert.SerializeObject(obj, Formatting.None);
            }

            return jsonResponse;
        }

        public static Dictionary<string, object> ToDictionary(this Object obj)
        {
            string json = obj.ToJson();
            return json.ToObject<Dictionary<string, object>>() ?? new Dictionary<string, object>();
        }

        public static Expression<Func<T, bool>> TransformDictIntoLambdaFilter<T>(this Dictionary<string, object?> filters)
        {
            var objectName = typeof(T).Name;
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? combinedExpression = null;

            foreach (var filter in filters)
            {
                var propertyName = filter.Key;
                var propertyValue = filter.Value;

                var propertyInfo = typeof(T).GetProperty(propertyName);

                if (propertyInfo == null)
                {
                    Console.WriteLine($"Property '{propertyName}' does not exist on {objectName}. Skipping.");
                    continue;
                }

                var property = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(propertyValue);

                var valueExpression = propertyInfo.PropertyType.IsGenericType &&
                                      propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    ? (Expression)Expression.Convert(constant, propertyInfo.PropertyType)
                    : constant;

                var equalityExpression = Expression.Equal(property, valueExpression);

                combinedExpression = combinedExpression == null
                    ? equalityExpression
                    : Expression.AndAlso(combinedExpression, equalityExpression);
            }

            if (combinedExpression == null)
            {
                throw new ArgumentException("No valid filters were provided.");
            }

            return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
        }
    }
}
