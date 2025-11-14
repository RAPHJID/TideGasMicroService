namespace OrderService.Models.DTOs
{
    public class OrderReadDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public string CustomerName { get; set; }
        public string CylinderName { get; set; }

        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
