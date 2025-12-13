namespace InventoryService.Models.DTOs
{
    public class InventoryDto
    {
        public Guid CylinderId { get; set; }

        public string Size { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;

        public decimal QuantityAvailable { get; set; }

    }
}
