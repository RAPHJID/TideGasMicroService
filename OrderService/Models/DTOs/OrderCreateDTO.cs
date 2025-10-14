namespace OrderService.Models.DTOs
{
    public class OrderCreateDTO
    {
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
    }
}
