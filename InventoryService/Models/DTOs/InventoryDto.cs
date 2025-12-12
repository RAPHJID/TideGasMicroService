namespace InventoryService.Models.DTOs
{
    public class InventoryDto
    {
        public Guid cylinderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;

        public decimal QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
