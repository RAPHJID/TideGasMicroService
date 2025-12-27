namespace OrderService.Models.DTOs
{
    public class CreateTransactionDTO
    {
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public decimal Amount { get; set; }
    }
}
