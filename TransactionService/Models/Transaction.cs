using System;

namespace TransactionService.Models
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }

        public Guid CustomerId { get; set; }
        public Guid CylinderId { get; set; }

        public int Quantity { get; set; }
        
        public DateTime TransactionDate { get; set; }
    }
}
