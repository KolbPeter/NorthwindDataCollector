using Microsoft.EntityFrameworkCore;

namespace NorthwindDataCollector.Dtos
{
    public class EmployeeTerritory
    {
        public int EmployeeId { get; set; }
        public string TerritoryId { get; set; }
    }
}
