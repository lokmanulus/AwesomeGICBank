using AwsomeGICBank.Models;
using AwsomeGICBank.Contracts;


namespace AwsomeGICBank
{
    public class PrintService : IPrintService
    {
        private readonly IInterestService _interestService;

        public PrintService(IInterestService interestService)
        {
            _interestService = interestService;
        }

        public void PrintMonthlyStatement(Account account, int year, int month)
        {

            var transactions = account.Transactions
                .Where(t => t.AccountId == account.AccountId && t.Date.Year == year && t.Date.Month == month)
                .OrderBy(t => t.Date)
                .ToList();

            decimal balance = _interestService.GetStartingBalance(account, year, month);
            decimal totalInterest = _interestService.CalculateMonthlyInterest(account, year, month);

            // Print header
            Console.WriteLine($"Account: {account.AccountId}");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount | Balance |");

            // Print transactions
            foreach (var txn in transactions)
            {
                balance = txn.Type == "D" ? balance + txn.Amount : balance - txn.Amount;
                Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type}    | {txn.Amount,7:F2} | {balance,8:F2} |");
            }

            // Print interest transaction at month-end
            Console.WriteLine($"| {year}{month:D2}30 |             | I    | {totalInterest,7:F2} | {balance + totalInterest,8:F2} |");
        }
        public void PrintStatement(Account account)
        {
            Console.WriteLine($"Account: {account.AccountId}");
            Console.WriteLine("| Date     | Txn Id      | Type | Amount |");
            foreach (var txn in account.Transactions)
            {
                Console.WriteLine($"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type}    | {txn.Amount,7:F2} |");
            }
        }


    }
}
