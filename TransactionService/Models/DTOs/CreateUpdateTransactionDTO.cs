namespace TransactionService.Models.DTOs
{
    public class CreateUpdateTransactionDTO
    {
        public string CustomerName { get; set; }
        public string CylinderName { get; set; }
        public decimal TotalPrice { get; set; } // cashier sets this
    }
}
