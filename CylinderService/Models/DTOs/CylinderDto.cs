namespace CylinderService.Models.DTOs
{
    public class CylinderDto
    {
        public Guid Id { get; set; }
        public string? Size { get; set; }
        public string? ImageUrl { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;
        public int SoldToday { get; set; }        
        public int TotalStock { get; set; }        
        public DateTime? LastUpdatedAt { get; set; } 
    }
}
