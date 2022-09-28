#pragma warning disable CS8618

namespace NorthwindDataCollector.NoSqlDocuments
{
    public record Product : Entity
    {
        public string ProductName { get; init; }
        public Supplier? Supplier { get; init; }
        public Category? Category { get; init; }
        public string? QuantityPerUnit { get; init; }
        public decimal? UnitPrice { get; init; }
        public short? UnitsInStock { get; init; }
        public short? UnitsOnOrder { get; init; }
        public short? ReorderLevel { get; init; }
        public bool Discontinued { get; init; }
    }
}
