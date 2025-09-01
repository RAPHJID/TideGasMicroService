namespace TransactionService.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid CylinderId { get; set; }
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
