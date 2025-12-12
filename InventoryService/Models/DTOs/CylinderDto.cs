namespace InventoryService.Models.DTOs
{
    public class CylinderDto
    {
        public Guid Id { get; set; }
        public string? Size { get; set; }
        public string? Name { get; set; }

        public string Brand { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;

    }
}
