#pragma warning disable CS8618

namespace NorthwindDataCollector.NoSqlDocuments
{
    public record EmployeeTerritory
    {
        public string TerritoryId { get; set; }
        public string TerritoryDescription { get; init; }
        public string RegionDescription { get; init; }
    }
}
