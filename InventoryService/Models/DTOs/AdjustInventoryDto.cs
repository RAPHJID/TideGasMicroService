namespace InventoryService.Models.DTOs
{
    public class AdjustInventoryDto
    {
        public Guid CylinderId { get; set; }
        public decimal QuantityChange { get; set; }
    }
}
