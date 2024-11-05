using AwsomeGICBank;
using AwsomeGICBank.Contracts;
using AwsomeGICBank.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

public class PrintServiceTests
{
    private readonly IInterestService _interestService;
    private readonly IPrintService _printService;

    public PrintServiceTests()
    {
        _interestService = new InterestService();
        _printService = new PrintService(_interestService);
    }

    [Fact]
    public void PrintMonthlyStatement_ShouldPrintCorrectStatement()
    {
        // Arrange
        var account = new Account
        {
            AccountId = "AC001",
            Transactions = new List<Transaction>
            {
            new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230505", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "D", Amount = 100m },
            new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230601", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "D", Amount = 150m },
            new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "W", Amount = 20m },
            new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "W", Amount = 100m }
            }
        };

        // Add interest rules
        var date1 = DateTime.ParseExact("20230101", "yyyyMMdd", CultureInfo.InvariantCulture);
        var date2 = DateTime.ParseExact("20230520", "yyyyMMdd", CultureInfo.InvariantCulture);
        var date3 = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);

        _interestService.AddOrUpdateInterestRule(date1, "RULE01", 1.95m);
        _interestService.AddOrUpdateInterestRule(date2, "RULE02", 1.90m);
        _interestService.AddOrUpdateInterestRule(date3, "RULE03", 2.20m);

        // Capture console output
        var output = CaptureConsoleOutput(() => _printService.PrintMonthlyStatement(account, 2023, 6));

        // Assert
        Assert.Contains("Account: AC001", output);
        Assert.Contains("| 20230601 |  | D    |  150.00 |   250.00 |", output);
        Assert.Contains("| 20230626 |  | W    |   20.00 |   230.00 |", output);
        Assert.Contains("| 20230626 |  | W    |  100.00 |   130.00 |", output);
        Assert.Contains("| 20230630 |             | I    |    0.39 |   130.39 |", output);
    }

    private string CaptureConsoleOutput(Action action)
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            action.Invoke();
            return sw.ToString();
        }
    }
}
