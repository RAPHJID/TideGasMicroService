namespace OrderService.Models.DTOs
{
    public class TransactionReadDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
