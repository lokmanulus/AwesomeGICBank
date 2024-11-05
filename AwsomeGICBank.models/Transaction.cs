namespace AwsomeGICBank.Models
{
    public class Transaction
    {
        public string AccountId { get; set; }
        public DateTime Date { get; set; }
        public string TransactionId { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
    }
}
