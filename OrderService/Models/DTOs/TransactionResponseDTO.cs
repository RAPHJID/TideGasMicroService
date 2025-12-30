namespace OrderService.Models.DTOs
{
    public class TransactionResponseDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public decimal Amount { get; set; }

        // Added for enrichment
        public string? CustomerName { get; set; }
        public string? CylinderName { get; set; }
    }
}
