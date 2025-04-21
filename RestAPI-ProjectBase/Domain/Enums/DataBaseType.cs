using System.Text.Json.Serialization;

namespace Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum DataBaseType
    {
        SQLSERVER,
        ORACLE,
        MYSQL,
        MARIADB,
        POSTGRESQL,
        FIREBIRD
    }
}
