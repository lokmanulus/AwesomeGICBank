using AwsomeGICBank.Models;
using System;
using System.Collections.Generic;

namespace AwsomeGICBank.Contracts
{
    public interface IAccountService
    {
        /// <summary>
        /// Starts the interactive process for inputting transactions.
        /// </summary>
        void InputTransaction();

        /// <summary>
        /// Adds or updates a transaction for an account.
        /// </summary>
        /// <param name="date">The date of the transaction.</param>
        /// <param name="accountId">The account ID for the transaction.</param>
        /// <param name="type">The type of transaction: "D" for deposit or "W" for withdrawal.</param>
        /// <param name="amount">The transaction amount.</param>
        /// <returns>True if the transaction was added or updated successfully; false otherwise.</returns>
        bool AddOrUpdateTransaction(DateTime date, string accountId, string type, decimal amount);

        /// <summary>
        /// Checks if an account exists.
        /// </summary>
        /// <param name="accountId">The account ID to check.</param>
        /// <returns>True if the account exists; otherwise, false.</returns>
        bool AccountExists(string accountId);

        /// <summary>
        /// Gets the current balance of an account.
        /// </summary>
        /// <param name="accountId">The account ID to get the balance for.</param>
        /// <returns>The balance of the account, or 0.0 if the account does not exist.</returns>
        decimal GetBalance(string accountId);

        /// <summary>
        /// Retrieves the transaction history for a specific account.
        /// </summary>
        /// <param name="accountId">The account ID to retrieve the transaction history for.</param>
        /// <returns>A list of transactions for the account.</returns>
        List<Transaction> GetTransactionHistory(string accountId);

        /// <summary>
        /// Gets the account statement as a formatted string.
        /// </summary>
        /// <param name="accountId">The account ID to get the statement for.</param>
        /// <returns>A string representing the account statement, or an error message if the account does not exist.</returns>
        string GetStatement(string accountId);

        /// <summary>
        /// returns the account
        /// </summary>
        /// <param name="accountId">The account ID to get </param>
        Account GetAccount(string accountId);
        
        }
}
