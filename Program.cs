using NorthwindDataCollector.Dtos;
using NorthwindDataCollector.NoSqlDocuments;
using System.Text.Json;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using Category = NorthwindDataCollector.Dtos.Category;
using Customer = NorthwindDataCollector.Dtos.Customer;
using Employee = NorthwindDataCollector.Dtos.Employee;
using EmployeeTerritory = NorthwindDataCollector.Dtos.EmployeeTerritory;
using OrderDetail = NorthwindDataCollector.Dtos.OrderDetail;
using Product = NorthwindDataCollector.Dtos.Product;
using Shipper = NorthwindDataCollector.Dtos.Shipper;
using Supplier = NorthwindDataCollector.Dtos.Supplier;

namespace NorthwindDataCollector
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var context = new DatabaseContext();
            var provider = new EntityProvider();

            var categories = provider.GetAll<Category>(context);
            var customers = provider.GetAll<Customer>(context);
            var customerCustomerDemo = provider.GetAll<CustomerCustomerDemo>(context);
            var customerDemographic = provider.GetAll<CustomerDemographic>(context);
            var employees = provider.GetAll<Employee>(context);
            var employeeTerritories = provider.GetAll<EmployeeTerritory>(context);
            var orders = provider.GetAll<Order>(context);
            var orderDetails = provider.GetAll<OrderDetail>(context);
            var products = provider.GetAll<Product>(context);
            var regions = provider.GetAll<Region>(context);
            var shippers = provider.GetAll<Shipper>(context);
            var suppliers = provider.GetAll<Supplier>(context);
            var territories = provider.GetAll<Territory>(context);

            var productsByOrderId = orderDetails
                .GroupBy(x => x.OrderId, (a, b) => new{ OrderId = a, ProductIds = b.Select(c => c.ProductId).ToArray()})
                .Select(x => new{ OrderId = x.OrderId, Products = products.Where(y => x.ProductIds.Contains(y.ProductId))})
                .Select(x => new{ OrderId = x.OrderId, Products = x.Products.Select(y => y.ToDocument(suppliers, categories))})
                .ToArray();

            var noSqlTerritories = territories
                .Select(x => new NoSqlDocuments.EmployeeTerritory()
                {
                    TerritoryId = x.TerritoryId,
                    TerritoryDescription = x.TerritoryDescription,
                    RegionDescription = regions.Single(y => y.RegionId == x.RegionId).RegionDescription
                });

            var orderAggregate = orders.Select(order => new OrderAggregate() with
            {
                OrderDate = order.OrderDate,
                RequiredDate = order.RequiredDate,
                ShippedDate = order.ShippedDate,
                Freight = order.Freight,
                ShipName = order.ShipName,
                ShipAddress = order.ShipAddress,
                ShipCity = order.ShipCity,
                ShipRegion = order.ShipRegion,
                ShipPostalCode = order.ShipPostalCode,
                ShipCountry = order.ShipCountry,
                ShipVia = shippers.SingleOrDefault(x => x.ShipperId == order.ShipVia)?.ToDocument() ?? null,
                OrderDetails = orderDetails.Where(x => x.OrderId == order.OrderId).Select(x => x.ToDocument(products, suppliers, categories)),
                Customer = customers.SingleOrDefault(x => x.CustomerID == order.CustomerId)?.ToDocument(customerCustomerDemo, customerDemographic),
                Employee = employees.SingleOrDefault(x => x.EmployeeId == order.EmployeeId)?.ToDocument(employeeTerritories, noSqlTerritories),
            })
                .ToArray();

            var path = Path.Combine(Environment.CurrentDirectory, "Results");
            Directory.CreateDirectory(path);

            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };

            var task1 = WriteToJson(orderAggregate, path, "Orders", options);
            var task2 = WriteToJson(categories.Select(x => x.ToDocument()), path, "Categories", options);
            var task3 = WriteToJson(customers.Select(x => x.ToDocument(customerCustomerDemo, customerDemographic)), path, "Customers", options);
            var task4 = WriteToJson(employees.Select(x => x.ToDocument(employeeTerritories, noSqlTerritories)), path, "Employees", options);
            var task5 = WriteToJson(products.Select(x => x.ToDocument(suppliers, categories)), path, "Products", options);
            var task6 = WriteToJson(shippers.Select(x => x.ToDocument()), path, "Shippers", options);
            var task7 = WriteToJson(suppliers.Select(x => x.ToDocument()), path, "Suppliers", options);

            Task.WaitAll(new[] { task1, task2, task3, task4, task5, task6, task7 });
        }

        private static async Task WriteToJson<T>(
            IEnumerable<T> data,
            string path,
            string filename,
            JsonSerializerOptions options)
            where T : Entity
        {
            
            Console.WriteLine($"Started writing {filename}.json");
            await File.WriteAllLinesAsync(
                path: Path.Combine(path, $"{filename}.json"),
                contents: data
                    .Select((x, i) =>
                    {
                        x._id = new IdClass { Id = i.ToString("X24") };
                            return x;
                        })
                    .Select(x => JsonSerializer.Serialize(x))
                    .ToArray());
            Console.WriteLine($"Finished writing {filename}.json");
        }
    }
}