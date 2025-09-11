namespace TransactionService.Models.DTOs
{
    public class TransactionResponseDTO
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty; 
        public string CylinderName { get; set; } = string.Empty;  
        public decimal TotalPrice { get; set; }   // Quantity × Cylinder.Price
        public DateTime TransactionDate { get; set; }
    }
}
