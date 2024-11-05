using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwsomeGICBank.Models
{
    public class Account
    {
        public string AccountId { get; set; }
        public decimal Balance { get; private set; }
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();

        public void AddTransaction(Transaction transaction)
        {
            if (transaction.Type == "D")
            {
                Balance += transaction.Amount;
            }
            else if (transaction.Type == "W" && Balance >= transaction.Amount)
            {
                Balance -= transaction.Amount;
            }
            Transactions.Add(transaction);
        }
    }
}
