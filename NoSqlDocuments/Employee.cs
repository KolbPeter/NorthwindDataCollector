#pragma warning disable CS8618

namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Employee : Entity
    {
        public string LastName { get; init; }
        public string FirstName { get; init; }
        public string? Title { get; init; }
        public string? TitleOfCourtesy { get; init; }
        public DateTime? BirthDate { get; init; }
        public DateTime? HireDate { get; init; }
        public string? Address { get; init; }
        public string? City { get; init; }
        public string? Region { get; init; }
        public string? PostalCode { get; init; }
        public string? Country { get; init; }
        public string? HomePhone { get; init; }
        public string? Extension { get; init; }
        public string? Photo { get; init; }
        public string? Notes { get; init; }
        public int? ReportsTo { get; init; }
        public string? PhotoPath { get; init; }
        public IEnumerable<EmployeeTerritory> EmployeeTerritories { get; init; }
    }
}
