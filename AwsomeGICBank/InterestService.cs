using AwsomeGICBank.Models;
using AwsomeGICBank.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class InterestService : IInterestService
{
    private readonly IDictionary<DateTime, InterestRule> _interestRules = new Dictionary<DateTime, InterestRule>();

    public void DefineInterestRule()
    {
        while (true)
        {
            Console.WriteLine("Please enter interest rules details in <Date> <RuleId> <Rate in %> format (or enter blank to go back to main menu):");
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input)) break;

            var parts = input.Split();
            if (parts.Length < 3 || !DateTime.TryParseExact(parts[0], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                || !decimal.TryParse(parts[2], out var rate) || rate <= 0 || rate >= 100)
            {
                Console.WriteLine("Invalid input format. Please try again.");
                continue;
            }

            var ruleId = parts[1];
            AddOrUpdateInterestRule(date, ruleId, rate);
            PrintInterestRules();
        }
    }

    public void AddOrUpdateInterestRule(DateTime date, string ruleId, decimal rate)
    {

        if (rate < 0 || rate > 100) {
            Console.WriteLine("Invalid rate");
            return;
        }

        _interestRules[date] = new InterestRule
        {
            Date = date,
            RuleId = ruleId,
            Rate = rate
        };
    }

    public InterestRule GetApplicableRule(DateTime date)
    {
        return _interestRules
            .Where(r => r.Key <= date)
            .OrderByDescending(r => r.Key)
            .Select(r => r.Value)
            .FirstOrDefault();
    }


    public decimal CalculateMonthlyInterest(Account account, int year, int month)
    {
        var dateRanges = GenerateInterestDateRanges(account, year, month);
        decimal totalInterest = 0m;

        foreach (var (startDate, endDate, balance, rule) in dateRanges)
        {
            int days = (endDate - startDate).Days + 1;
            decimal periodInterest = (balance * rule.Rate / 100) * (days / 365m);
            totalInterest += periodInterest;
        }

        return Math.Round(totalInterest, 2);
    }

    private List<(DateTime startDate, DateTime endDate, decimal balance, InterestRule rule)> GenerateInterestDateRanges(Account account, int year, int month)
    {
        var dateRanges = new List<(DateTime startDate, DateTime endDate, decimal balance, InterestRule rule)>();

        DateTime currentStartDate = new DateTime(year, month, 1);
        decimal balance = GetStartingBalance(account, year, month);
        var currentRule = GetApplicableRule(currentStartDate);

        // Collect all relevant dates: transaction dates and rule change dates within the month
        var relevantDates = account.Transactions
            .Where(t => t.Date.Year == year && t.Date.Month == month)  // Filter for transactions in the given month
            .Select(t => t.Date)
            .Concat(_interestRules.Keys.Where(d => d.Year == year && d.Month == month))  // Add rule change dates within the month
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        foreach (var date in relevantDates)
        {
            // Set the end date to the day before the current date
            DateTime currentEndDate = date.AddDays(-1);

            // Add the date range if it's valid
            if (currentEndDate >= currentStartDate)
            {
                dateRanges.Add((currentStartDate, currentEndDate, balance, currentRule));
            }

            // Update balance only for transactions that occur within the month
            var transactionsOnDate = account.Transactions.Where(t => t.Date == date);
            foreach (var txn in transactionsOnDate)
            {
                balance = txn.Type == "D" ? balance + txn.Amount : balance - txn.Amount;
            }

            // Update the interest rule if there's a rule change on this date
            var ruleChange = GetApplicableRule(date);
            if (ruleChange != null)
            {
                currentRule = ruleChange;
            }

            // Move the start date to the current date
            currentStartDate = date;
        }

        // Add the final range to the end of the month
        DateTime monthEnd = new DateTime(year, month, DateTime.DaysInMonth(year, month));
        if (currentStartDate <= monthEnd)
        {
            dateRanges.Add((currentStartDate, monthEnd, balance, currentRule));
        }

        return dateRanges;
    }



    private decimal GetBalanceForTransaction(decimal currentBalance, Transaction txn)
    {
        return txn.Type == "D" ? currentBalance + txn.Amount : currentBalance - txn.Amount;
    }

    public decimal GetStartingBalance(Account account, int year, int month)
    {
        return account.Transactions
             .Where(t => t.Date < new DateTime(year, month, 1))
             .Aggregate(0m, (balance, txn) =>
                 txn.Type == "D" ? balance + txn.Amount : balance - txn.Amount);
    }

    public void PrintInterestRules()
    {
        Console.WriteLine("Interest rules:");
        Console.WriteLine("| Date     | RuleId | Rate (%) |");

        foreach (var rule in _interestRules.OrderBy(r => r.Key))
        {
            Console.WriteLine($"| {rule.Value.Date:yyyyMMdd} | {rule.Value.RuleId} | {rule.Value.Rate,8:F2} |");
        }
    }
}
