
namespace InventoryService.Models
{
    public class Inventory
    {
        public Guid Id { get; set; }

        // Link to CylinderService
        public Guid CylinderId { get; set; }

        // Current stock
        public int QuantityAvailable { get; set; }

        // Audit fields
        public DateTime LastUpdated { get; set; }

        // WHO updated inventory (staff/admin user id from JWT)
        public string UpdatedByUserId { get; set; } = string.Empty;
    }
}

