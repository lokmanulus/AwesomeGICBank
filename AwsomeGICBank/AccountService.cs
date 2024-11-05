

namespace AwsomeGICBank
{
    using AwsomeGICBank.Contracts;
    using AwsomeGICBank.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Transaction = Models.Transaction;

    public class AccountService : IAccountService
    {
        private readonly IPrintService _printService;

        private readonly IDictionary<string, Account> _accounts = new Dictionary<string, Account>();
        public AccountService(IPrintService printService)
        {
            _printService = printService;
        }


        public void InputTransaction()
        {
            while (true)
            {
                Console.WriteLine("Please enter transaction details in <Date> <Account> <Type> <Amount> format (or enter blank to go back to main menu):");
                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                var parts = input.Split();
                if (parts.Length < 4 || !DateTime.TryParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                    || !decimal.TryParse(parts[3], out var amount) || amount <= 0 || !(parts[2].Equals("D", StringComparison.OrdinalIgnoreCase) || parts[2].Equals("W", StringComparison.OrdinalIgnoreCase)))
                {
                    Console.WriteLine("Invalid input format. Please try again.");
                    continue;
                }

                var accountId = parts[1];
                var type = parts[2].ToUpper();
                AddOrUpdateTransaction(date, accountId, type, amount);
                _printService.PrintStatement(GetAccount(accountId));
            }
        }

        public bool AddOrUpdateTransaction(DateTime date, string accountId, string type, decimal amount)
        {
            if (!_accounts.ContainsKey(accountId))
            {
                if (type.Equals("W", StringComparison.OrdinalIgnoreCase))
                {
                    // Cannot start with a withdrawal
                    return false;
                }

                _accounts[accountId] = new Account { AccountId = accountId };
            }

            var account = _accounts[accountId];
            var transactionId = GenerateTransactionId(date, account);
            var transaction = new Transaction
            {
                Date = date,
                TransactionId = transactionId,
                Type = type,
                Amount = amount
            };

            // Check for withdrawal with insufficient funds
            if (type.Equals("W", StringComparison.OrdinalIgnoreCase) && account.Balance < amount)
            {
                return false;
            }

            account.AddTransaction(transaction);
            return true;
        }

        private string GenerateTransactionId(DateTime date, Account account)
        {
            int transactionCount = account.Transactions.Count(txn => txn.Date.Date == date.Date) + 1;
            return $"{date:yyyyMMdd}-{transactionCount:D2}";
        }


        public bool AccountExists(string accountId)
        {
            return _accounts.ContainsKey(accountId);
        }

        public decimal GetBalance(string accountId)
        {
            if (_accounts.ContainsKey(accountId))
            {
                return _accounts[accountId].Balance;
            }
            Console.WriteLine("Account does not exist.");
            return 0.0m;
        }

        public List<Transaction> GetTransactionHistory(string accountId)
        {
            if (_accounts.ContainsKey(accountId))
            {
                return _accounts[accountId].Transactions;
            }
            Console.WriteLine("Account does not exist.");
            return new List<Transaction>();
        }

        public string GetStatement(string accountId)
        {
            if (!_accounts.ContainsKey(accountId))
            {
                return "Account does not exist.";
            }

            var account = _accounts[accountId];
            var statement = $"Account: {accountId}\n";
            statement += "| Date     | Txn Id      | Type | Amount |\n";

            foreach (var txn in account.Transactions)
            {
                statement += $"| {txn.Date:yyyyMMdd} | {txn.TransactionId} | {txn.Type}    | {txn.Amount,7:F2} |\n";
            }

            return statement;
        }


        public Account GetAccount(string accountId) {
            if (!_accounts.ContainsKey(accountId))
            {
                Console.WriteLine("Account does not exist.");
                return null;
            }
            return _accounts[accountId];
        }


    }

}
