namespace TransactionService.Models.DTOs
{
    public class TransactionResponseDTO
    {
        public Guid Id { get; set; }
        public string? CylinderName { get; set; }   
        public int Quantity { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
