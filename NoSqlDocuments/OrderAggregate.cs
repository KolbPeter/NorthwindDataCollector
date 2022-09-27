namespace NorthwindDataCollector.NoSqlDocuments
{
    public record OrderAggregate : Entity
    {
        public IEnumerable<OrderDetail>? OrderDetails { get; init; }
        public Customer? Customer { get; init; }
        public Employee? Employee { get; init; }
        public DateTime? OrderDate { get; init; }
        public DateTime? RequiredDate { get; init; }
        public DateTime? ShippedDate { get; init; }
        public Shipper? ShipVia { get; init; }
        public decimal? Freight { get; init; }
        public string? ShipName { get; init; }
        public string? ShipAddress { get; init; }
        public string? ShipCity { get; init; }
        public string? ShipRegion { get; init; }
        public string? ShipPostalCode { get; init; }
        public string? ShipCountry { get; init; }
    }
}
