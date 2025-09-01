namespace TransactionService.Models.DTOs
{
    public class CreateUpdateTransactionDTO
    {
        public Guid CylinderId { get; set; }  
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
