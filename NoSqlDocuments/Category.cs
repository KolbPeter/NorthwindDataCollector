namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Category : Entity
    {
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string? Picture { get; set; }
    }
}
