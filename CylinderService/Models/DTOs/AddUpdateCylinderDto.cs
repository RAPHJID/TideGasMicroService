namespace CylinderService.Models.DTOs
{
    public class AddUpdateCylinderDto
    {
        public string? Size { get; set; }

        public string Brand { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
    }
}
