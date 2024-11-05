using AwsomeGICBank;
using System;
using System.Globalization;
using Xunit;

namespace AwesomeGICBank.Tests
{
    public class AccountServiceTests
    {
        private readonly AccountService _accountService;

        public AccountServiceTests()
        {
            _accountService = new AccountService(new PrintService(new InterestService()));
        }

        [Fact]
        public void DepositTransaction_CreatesAccountAndUpdatesBalance()
        {
            // Arrange
            var date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";
            var type = "D";
            var amount = 100.00m;

            // Act
            _accountService.AddOrUpdateTransaction(date, accountId, type, amount);

            // Assert
            Assert.True(_accountService.AccountExists(accountId));
            Assert.Equal(100.00m, _accountService.GetBalance(accountId));
        }

        [Fact]
        public void WithdrawTransaction_UpdatesBalance()
        {
            var date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";
            _accountService.AddOrUpdateTransaction(date, accountId, "D", 200.00m);

            _accountService.AddOrUpdateTransaction(date, accountId, "W", 50.00m);

            Assert.Equal(150.00m, _accountService.GetBalance(accountId));
        }

        [Fact]
        public void WithdrawTransaction_InsufficientFunds_DoesNotAllowWithdrawal()
        {
            var date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";
            _accountService.AddOrUpdateTransaction(date, accountId, "D", 50.00m); 

            var result = _accountService.AddOrUpdateTransaction(date, accountId, "W", 100.00m);

            Assert.False(result); 
            Assert.Equal(50.00m, _accountService.GetBalance(accountId)); 
        }

        [Fact]
        public void FirstTransactionCannotBeWithdrawal()
        {
            // Arrange
            var date = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";

            // Act
            var result = _accountService.AddOrUpdateTransaction(date, accountId, "W", 50.00m);

            // Assert
            Assert.False(result); // Should return false as first transaction cannot be withdrawal
            Assert.Equal(0.00m, _accountService.GetBalance(accountId)); // Balance should remain zero
        }

        [Fact]
        public void TransactionHistory_IsRecordedCorrectly()
        {
            // Arrange
            var date1 = DateTime.ParseExact("20230601", "yyyyMMdd", CultureInfo.InvariantCulture);
            var date2 = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";
            _accountService.AddOrUpdateTransaction(date1, accountId, "D", 100.00m);
            _accountService.AddOrUpdateTransaction(date2, accountId, "W", 20.00m);

            // Act
            var transactions = _accountService.GetTransactionHistory(accountId);

            // Assert
            Assert.Equal(2, transactions?.Count);
            Assert.Equal("D", transactions[0].Type);
            Assert.Equal("W", transactions[1].Type);
            Assert.Equal(100.00m, transactions[0].Amount);
            Assert.Equal(20.00m, transactions[1].Amount);
        }

        [Fact]
        public void PrintStatement_GeneratesCorrectFormat()
        {
            // Arrange
            var date1 = DateTime.ParseExact("20230601", "yyyyMMdd", CultureInfo.InvariantCulture);
            var date2 = DateTime.ParseExact("20230626", "yyyyMMdd", CultureInfo.InvariantCulture);
            var accountId = "AC001";
            _accountService.AddOrUpdateTransaction(date1, accountId, "D", 100.00m);
            _accountService.AddOrUpdateTransaction(date2, accountId, "W", 20.00m);

            // Act
            var statement = _accountService.GetStatement(accountId);

            // Assert
            Assert.Contains("Account: AC001", statement);
            Assert.Contains("| Date     | Txn Id      | Type | Amount |", statement);
            Assert.Contains("| 20230601 | 20230601-01 | D    |  100.00 |", statement);
            Assert.Contains("| 20230626 | 20230626-01 | W    |   20.00 |", statement);
        }
    }
}
