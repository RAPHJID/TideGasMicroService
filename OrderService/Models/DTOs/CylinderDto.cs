namespace OrderService.Models.DTOs
{
    public class CylinderDto
    {
        public Guid Id { get; set; }
        public string Size { get; set; } = string.Empty;
        public string Brand { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? Condition { get; set; }
    }
}
