namespace InventoryService.Models.DTOs
{
    public class InventoryDto
    {
        public Guid Id { get; set; }
        public decimal QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
