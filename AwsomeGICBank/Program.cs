using AwsomeGICBank;
using AwsomeGICBank.Contracts;
using System;

namespace AwesomeGICBank
{
    public class Program
    {
        private static IInterestService _interestService = new InterestService();
        private static IPrintService _printService = new PrintService(_interestService);
        private static IAccountService _accountService = new AccountService(_printService);

        public static void Main()
        {
            while (true)
            {
                Console.WriteLine("Welcome to AwesomeGIC Bank! What would you like to do?");
                Console.WriteLine("[T] Input transactions");
                Console.WriteLine("[I] Define interest rules");
                Console.WriteLine("[P] Print statement");
                Console.WriteLine("[Q] Quit");
                Console.Write("> ");
                var choice = Console.ReadLine()?.ToUpper();

                switch (choice)
                {
                    case "T":
                        _accountService.InputTransaction();
                        break;
                    case "I":
                        _interestService.DefineInterestRule();
                        break;
                    case "P":
                        PrintStatement(); ;
                        break;
                    case "Q":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void PrintStatement()
        {
            Console.Write("Please enter account and month to generate the statement <Account> <Year><Month> (or enter blank to go back to main menu): ");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split();
            if (parts.Length != 2 || !int.TryParse(parts[1].Substring(0, 4), out int year) || !int.TryParse(parts[1].Substring(4, 2), out int month))
            {
                Console.WriteLine("Invalid format. Please enter <Account> <Year><Month>.");
                return;
            }

            var accountId = parts[0];
            var account = _accountService.GetAccount(accountId);
            if (accountId == null) return;
            _printService.PrintMonthlyStatement(account, year, month);
        }
    }
}
