using NorthwindDataCollector.Dtos;
using NorthwindDataCollector.NoSqlDocuments;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
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
            await using var context = new DatabaseContext();
            var provider = new EntityProvider();
            var resultPath = PrepareFolder();

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

            var noSqlTerritories = territories
                .Select(x => new NoSqlDocuments.EmployeeTerritory()
                {
                    TerritoryId = x.TerritoryId,
                    TerritoryDescription = x.TerritoryDescription,
                    RegionDescription = regions.Single(y => y.RegionId == x.RegionId).RegionDescription
                });

            JsonSerializerOptions options = new()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };

            var tasks = new[]
            {
                WriteToJson(
                    data: orders.Select(x => x.ToDocument(shippers, orderDetails, products, suppliers, categories, customers, employees, customerCustomerDemo, customerDemographic, employeeTerritories, noSqlTerritories)),
                    path: resultPath,
                    filename: "Orders",
                    options: options),
                WriteToJson(
                    data: categories.Select(x => x.ToDocument()),
                    resultPath,
                    filename: "Categories",
                    options: options),
                WriteToJson(
                    data: customers.Select(x => x.ToDocument(customerCustomerDemo, customerDemographic)),
                    resultPath,
                    filename: "Customers",
                    options: options),
                WriteToJson(
                    data: employees.Select(x => x.ToDocument(employeeTerritories, noSqlTerritories)),
                    resultPath,
                    filename: "Employees",
                    options: options),
                WriteToJson(
                    data: products.Select(x => x.ToDocument(suppliers, categories)),
                    resultPath,
                    filename: "Products",
                    options: options),
                WriteToJson(
                    data: shippers.Select(x => x.ToDocument()),
                    resultPath,
                    filename: "Shippers",
                    options: options),
                WriteToJson(
                    data: suppliers.Select(x => x.ToDocument()),
                    resultPath,
                    filename: "Suppliers",
                    options: options),
            };

            Task.WaitAll(tasks);

            BuildImage();
        }

        private static string PrepareFolder()
        {
            var resultPath = Path.Combine(Environment.CurrentDirectory, "Result");
            Directory.CreateDirectory(resultPath);

            var setupFileName = "mysetup.sh";
            if (!File.Exists(Path.Combine(resultPath, setupFileName)))
            {
                File.Copy(Path.Combine(Environment.CurrentDirectory, setupFileName), Path.Combine(resultPath, setupFileName));
            }

            return resultPath;
        }

        private static void BuildImage()
        {
            var dockerfilePath = Path.Combine(Environment.CurrentDirectory, ".");
            var imageTag = "kolbpeter/mongodb:northwind";
            var buildImageCommand = $"docker build -t {imageTag} {dockerfilePath}";

            using var process = new Process();
            process.StartInfo = new ProcessStartInfo
            {
                CreateNoWindow = false,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Normal,
                Arguments = $"/C {buildImageCommand}", // $"{buildImageCommand}"
                FileName = "cmd.exe", // "/bin/bash"
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Image build failed with exit code {process.ExitCode}");
            }
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