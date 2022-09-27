using MongoDB.Bson.Serialization.IdGenerators;
using NorthwindDataCollector.Dtos;
using NorthwindDataCollector.NoSqlDocuments;
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
    public static class ExtensionMethods
    {
        public static NoSqlDocuments.Shipper ToDocument(this Shipper dto) =>
            new()
            {
                CompanyName = dto.CompanyName,
                Phone = dto.Phone
            };

        public static NoSqlDocuments.Category ToDocument(this Category dto) =>
            new()
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description,
                Picture = dto.Picture == null ? null : Convert.ToBase64String(dto.Picture)
            };

        public static NoSqlDocuments.Supplier ToDocument(this Supplier dto) =>
            new()
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                ContactTitle = dto.ContactTitle,
                Address = dto.Address,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Phone = dto.Phone,
                Fax = dto.Fax,
                HomePage = dto.HomePage
            };

        public static NoSqlDocuments.OrderDetail ToDocument(
            this OrderDetail dto,
            IEnumerable<Product> products,
            IEnumerable<Supplier> suppliers,
            IEnumerable<Category> categories) =>
            new ()
            {
                Product = products
                    .First(x => x.ProductId == dto.ProductId)
                    .ToOrderProduct(suppliers, categories),
                UnitPrice = dto.UnitPrice,
                Discount = dto.Discount,
                Quantity = dto.Quantity,
            };

        private static NoSqlDocuments.OrderProduct ToOrderProduct(
            this Product product,
            IEnumerable<Supplier> suppliers,
            IEnumerable<Category> categories) =>
            new ()
            {
                ProductName = product.ProductName,
                Supplier = suppliers
                    .FirstOrDefault(x => x.SupplierId == product.SupplierId)?
                    .ToDocument(),
                Category = categories
                    .FirstOrDefault(x => x.CategoryId == product.CategoryId)?
                    .ToDocument(),
                QuantityPerUnit = product.QuantityPerUnit,
                UnitPrice = product.UnitPrice,
            };

        public static NoSqlDocuments.Product ToDocument(
            this Product dto,
            IEnumerable<Supplier> suppliers,
            IEnumerable<Category> categories) =>
            new()
            {
                ProductName = dto.ProductName,
                Supplier = suppliers.SingleOrDefault(x => x.SupplierId == dto.SupplierId)?.ToDocument() ?? null,
                Category = categories.SingleOrDefault(x => x.CategoryId == dto.CategoryId)?.ToDocument() ?? null,
                QuantityPerUnit = dto.QuantityPerUnit,
                UnitPrice = dto.UnitPrice,
                UnitsInStock = dto.UnitsInStock,
                UnitsOnOrder = dto.UnitsOnOrder,
                ReorderLevel = dto.ReorderLevel,
                Discontinued = dto.Discontinued
            };

        public static NoSqlDocuments.Customer ToDocument(
            this Customer dto,
            IEnumerable<CustomerCustomerDemo> customerCustomerDemos,
            IEnumerable<CustomerDemographic> customerDemographics)
        {
            var customerTypeIds = customerCustomerDemos
                        .Where(x => x.CustomerId == dto.CustomerID)
                        .Select(x => x.CustomerTypeId);

            var descriptions = customerTypeIds == null
                ? null
                : customerDemographics
                    .Where(x => customerTypeIds.Contains(x.CustomerTypeId))
                    .Select(x => x.CustomerDesc ?? string.Empty);

            return new()
            {
                CompanyName = dto.CompanyName,
                ContactName = dto.ContactName,
                ContactTitle = dto.ContactTitle,
                Address = dto.Address,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                Phone = dto.Phone,
                Fax = dto.Fax,
                Description = descriptions,
            };
        }

        public static NoSqlDocuments.Employee ToDocument(
            this Employee dto,
            IEnumerable<EmployeeTerritory> employeeTerritories,
            IEnumerable<NoSqlDocuments.EmployeeTerritory> territories) =>
            new()
            {
                LastName = dto.LastName,
                FirstName = dto.FirstName,
                Title = dto.Title,
                TitleOfCourtesy = dto.TitleOfCourtesy,
                BirthDate = dto.BirthDate,
                HireDate = dto.HireDate,
                Address = dto.Address,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                HomePhone = dto.HomePhone,
                Extension = dto.Extension,
                Photo = dto.Photo == null ? null : Convert.ToBase64String(dto.Photo),
                Notes = dto.Notes,
                ReportsTo = dto.ReportsTo,
                PhotoPath = dto.PhotoPath,
                EmployeeTerritories = territories
                    .Where(x => employeeTerritories
                        .Where(y => y.EmployeeId == dto.EmployeeId)
                        .Select(y => y.TerritoryId)
                        .Contains(x.TerritoryId))
            };

        public static OrderAggregate ToDocument(
            this Order dto,
            IEnumerable<Shipper> shippers,
            IEnumerable<OrderDetail> orderDetails,
            IEnumerable<Product> products,
            IEnumerable<Supplier> suppliers,
            IEnumerable<Category> categories,
            IEnumerable<Customer> customers,
            IEnumerable<Employee> employees,
            IEnumerable<CustomerCustomerDemo> customerCustomerDemo,
            IEnumerable<CustomerDemographic> customerDemographic,
            IEnumerable<EmployeeTerritory> employeeTerritories,
            IEnumerable<NoSqlDocuments.EmployeeTerritory> noSqlTerritories) =>
            new()
            {
                OrderDate = dto.OrderDate,
                RequiredDate = dto.RequiredDate,
                ShippedDate = dto.ShippedDate,
                Freight = dto.Freight,
                ShipName = dto.ShipName,
                ShipAddress = dto.ShipAddress,
                ShipCity = dto.ShipCity,
                ShipRegion = dto.ShipRegion,
                ShipPostalCode = dto.ShipPostalCode,
                ShipCountry = dto.ShipCountry,
                ShipVia = shippers.SingleOrDefault(x => x.ShipperId == dto.ShipVia)?.ToDocument() ?? null,
                OrderDetails = orderDetails.Where(x => x.OrderId == dto.OrderId)
                    .Select(x => x.ToDocument(products, suppliers, categories)),
                Customer = customers.SingleOrDefault(x => x.CustomerID == dto.CustomerId)
                    ?.ToDocument(customerCustomerDemo, customerDemographic),
                Employee = employees.SingleOrDefault(x => x.EmployeeId == dto.EmployeeId)
                    ?.ToDocument(employeeTerritories, noSqlTerritories),
            };
    }
}