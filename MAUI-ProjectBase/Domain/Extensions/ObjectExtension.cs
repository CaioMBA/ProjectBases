using Newtonsoft.Json;
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
    }
}
