namespace TransactionService.Models.DTOs
{
    public class CreateUpdateTransactionDTO
    {
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public DateTime Date { get; set; } 
        public decimal Amount { get; set; }
    }
}
