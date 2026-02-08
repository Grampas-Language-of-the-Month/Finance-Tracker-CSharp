using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public void GenerateHash()
        {
            var rawData = $"{TransactionDate.ToShortDateString()}|{Description}|{Amount}";
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                Hash = Convert.ToBase64String(bytes);
            }
        }

        public Transaction(DateTime date, string description, decimal amount) 
        {
            TransactionDate = date;
            Description = description;
            Amount = amount;
            GenerateHash();
        }

        protected Transaction() { }
    }
}
