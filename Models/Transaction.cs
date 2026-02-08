using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace finance_tracker.Models
{
    public class Transaction
    {
        [Key]
        public string Hash { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public void GenerateHash()
        {
            var rawData = $"{TransactionDate.ToShortDateString()}|{Description}|{Amount}";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                Hash = Convert.ToBase64String(bytes);
            }
        }

        public Transaction(DateTime date, string description, decimal amount, string category) 
        {
            TransactionDate = date;
            Description = description;
            Amount = amount;
            Category = category;
            GenerateHash();
        }

        protected Transaction() { }
    }
}
