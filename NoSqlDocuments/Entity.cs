#pragma warning disable CS8618
using System.Text.Json.Serialization;

namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Entity
    {
        public IdClass _id { get; set; }
    }

    public record IdClass
    {
        [JsonPropertyName("$oid")]
        public string Id { get; set; }
    }
}
