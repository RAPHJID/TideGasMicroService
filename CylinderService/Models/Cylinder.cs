namespace CylinderService.Models
{
    public class Cylinder
    {
        public Guid Id { get; set; }

        public string? Size { get; set; }
        public string Brand { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Condition { get; set; } = string.Empty;

        public DateTime LastRefilled { get; set; }

        //INVENTORY
        public int TotalStock { get; set; }
        public int SoldToday { get; set; }

        //AUDIT(VERY IMPORTANT)
        public string? LastUpdatedByStaffId { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}
