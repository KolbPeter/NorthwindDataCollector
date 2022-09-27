namespace NorthwindDataCollector.NoSqlDocuments;

public record OrderProduct
{
    public string ProductName { get; init; }
    public Supplier? Supplier { get; init; }
    public Category? Category { get; init; }
    public string? QuantityPerUnit { get; init; }
    public decimal? UnitPrice { get; init; }
}