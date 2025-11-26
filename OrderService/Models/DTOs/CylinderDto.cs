namespace OrderService.Models.DTOs
{
    public class CylinderDto
    {
        public Guid cylinderId { get; set; }
        public string? CylinderName { get; set; }
        public int QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
