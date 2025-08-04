
using CylinderService.Models;

namespace InventoryService.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }

        public Guid CylinderId { get; set; }         
        public Cylinder? Cylinder { get; set; }       

        public decimal QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }


    }
}
