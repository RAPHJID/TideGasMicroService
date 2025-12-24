using System.ComponentModel.DataAnnotations;

namespace TransactionService.Models
{
    public class Transaction
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid CylinderId { get; set; }

        [Required]
        public DateTime Date { get; set; } 

        [Required]
        public decimal Amount { get; set; }
    }
}
