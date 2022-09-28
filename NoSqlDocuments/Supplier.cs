#pragma warning disable CS8618

namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Supplier : Entity
    {
        public string CompanyName { get; init; }
        public string? ContactName { get; init; }
        public string? ContactTitle { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? Region { get; init; }
        public string? PostalCode { get; init; }
        public string? Country { get; init; }
        public string? Phone { get; init; }
        public string? Fax { get; init; }
        public string? HomePage { get; init; }
    }
}
