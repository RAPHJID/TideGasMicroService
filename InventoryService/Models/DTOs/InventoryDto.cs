namespace InventoryService.Models.DTOs
{
    public class InventoryDto
    {
        public Guid cylinderId { get; set; }
        public string CylinderName { get; set; } = string.Empty;

        public decimal QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
