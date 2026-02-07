using System.ComponentModel.DataAnnotations;

namespace finance_tracker.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string ExternalReference { get; set; }
    }
}
