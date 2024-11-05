namespace AwsomeGICBank.tests
{
    using AwsomeGICBank.Contracts;
    using AwsomeGICBank.Models;
    using System;
    using System.Globalization;
    using Xunit;

    public class InterestServiceTests
    {
        private readonly IInterestService _interestService;

        public InterestServiceTests()
        {
            _interestService = new InterestService();
        }

        [Fact]
        public void AddOrUpdateInterestRule_ShouldAddNewRule()
        {
            // Arrange
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);
            var ruleId = "RULE03";
            var rate = 2.20m;

            // Act
            _interestService.AddOrUpdateInterestRule(date, ruleId, rate);

            // Assert
            var statement = CaptureInterestRulesOutput();
            Assert.Contains("| 20230615 | RULE03 |     2.20 |", statement);
        }

        [Fact]
        public void AddOrUpdateInterestRule_ShouldUpdateExistingRuleForSameDate()
        {
            // Arrange
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);
            _interestService.AddOrUpdateInterestRule(date, "RULE03", 2.20m); 

            // Act
            _interestService.AddOrUpdateInterestRule(date, "RULE03_UPDATED", 2.50m);

            // Assert
            var statement = CaptureInterestRulesOutput();
            Assert.Contains("| 20230615 | RULE03_UPDATED |     2.50 |", statement);
            Assert.DoesNotContain("| 20230615 | RULE03 |     2.20 |", statement); // Original rule should be removed
        }

        [Fact]
        public void AddOrUpdateInterestRule_ShouldHandleMultipleDates()
        {
            // Arrange
            var date1 = DateTime.ParseExact("20230101", "yyyyMMdd", CultureInfo.InvariantCulture);
            var date2 = DateTime.ParseExact("20230520", "yyyyMMdd", CultureInfo.InvariantCulture);
            var date3 = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);

            _interestService.AddOrUpdateInterestRule(date1, "RULE01", 1.95m);
            _interestService.AddOrUpdateInterestRule(date2, "RULE02", 1.90m);
            _interestService.AddOrUpdateInterestRule(date3, "RULE03", 2.20m);

            // Act
            var statement = CaptureInterestRulesOutput();

            // Assert
            Assert.Contains("| 20230101 | RULE01 |     1.95 |", statement);
            Assert.Contains("| 20230520 | RULE02 |     1.90 |", statement);
            Assert.Contains("| 20230615 | RULE03 |     2.20 |", statement);
        }

        [Fact]
        public void AddOrUpdateInterestRule_ShouldNotAddRuleWithInvalidRate()
        {
            // Arrange
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);
            var ruleId = "RULE_INVALID";
            var invalidRate = -1.00m;

            // Act
            var consoleOutput = TestHelper.CaptureConsoleOutput(() => _interestService.AddOrUpdateInterestRule(date, ruleId, invalidRate));

            // Assert
            Assert.Contains("Invalid rate", consoleOutput);
            var statement = CaptureInterestRulesOutput();
            Assert.DoesNotContain("RULE_INVALID", statement); // Ensure rule was not added
        }


        [Fact]
        public void AddOrUpdateInterestRule_ShouldNotAddRuleWithRateOver100()
        {
            // Arrange
            var date = DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture);
            var ruleId = "RULE_OVER100";
            var invalidRate = 100.50m;

            var consoleOutput = TestHelper.CaptureConsoleOutput(() => _interestService.AddOrUpdateInterestRule(date, ruleId, invalidRate));

            // Assert
            Assert.Contains("Invalid rate", consoleOutput);
            var statement = CaptureInterestRulesOutput();
            Assert.DoesNotContain("RULE_INVALID", statement);
        }

        [Fact]
        public void GetStartingBalance_ShouldCalculateCorrectBalance()
        {
            // Arrange
            var account = new Account
            {
                AccountId = "AC001",
                Transactions = new List<Transaction>
            {
                new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230601", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "D", Amount = 150m },
                new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230522", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "W", Amount = 50m },
                new Transaction { AccountId = "AC001", Date = DateTime.ParseExact("20230520", "yyyyMMdd", CultureInfo.InvariantCulture), Type = "D", Amount = 100m }
            }
            };

            // Act
            decimal startingBalance = _interestService.GetStartingBalance(account, 2023, 6);

            // Assert
            Assert.Equal(50m, startingBalance); // 150 - 50 + 100 = 200
        }

        [Fact]
        public void CalculateMonthlyInterest_ShouldCalculateCorrectInterest()
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
            _interestService.AddOrUpdateInterestRule(DateTime.ParseExact("20230101", "yyyyMMdd", CultureInfo.InvariantCulture), "RULE01", 1.95m);
            _interestService.AddOrUpdateInterestRule(DateTime.ParseExact("20230520", "yyyyMMdd", CultureInfo.InvariantCulture), "RULE02", 1.90m);
            _interestService.AddOrUpdateInterestRule(DateTime.ParseExact("20230615", "yyyyMMdd", CultureInfo.InvariantCulture), "RULE03", 2.20m);

            // Act
            decimal interest = _interestService.CalculateMonthlyInterest(account, 2023, 6); // Starting balance in June is 250

            // Assert
            Assert.Equal(0.39m, interest);
        }


        private string CaptureInterestRulesOutput()
        {
            using (var sw = new System.IO.StringWriter())
            {
                Console.SetOut(sw);
                _interestService.PrintInterestRules();
                return sw.ToString();
            }
        }
    }

}
