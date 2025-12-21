namespace OrderService.Models.DTOs
{
    public class CylinderDto
    {
        public Guid Id { get; set; }
        public string? Size { get; set; }
        public string Brand { get; set; } = string.Empty;
    }
}
