using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ApiProtocolType
    {
        REST,
        GraphQL,
        gRPC,
        SOAP
    }
}
