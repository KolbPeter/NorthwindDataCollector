namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Shipper : Entity
    {
        public string CompanyName { get; init; }
        public string? Phone { get; init; }
    }
}
        