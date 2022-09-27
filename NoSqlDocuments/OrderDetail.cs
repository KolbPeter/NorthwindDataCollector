namespace NorthwindDataCollector.NoSqlDocuments
{
    public record OrderDetail
    {
        public OrderProduct Product { get; init; }
        public decimal UnitPrice { get; init; }
        public short Quantity { get; init; }
        public float Discount { get; init; }
    }
}
