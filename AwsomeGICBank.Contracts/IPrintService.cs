using AwsomeGICBank.Models;
using System;

namespace AwsomeGICBank.Contracts
{
    public interface IPrintService
    {
        /// <summary>
        /// Prints the monthly statement for a specific account and month.
        /// </summary>
        /// <param name="account">The account for which the statement is to be printed.</param>
        /// <param name="year">The year of the statement.</param>
        /// <param name="month">The month of the statement.</param>
        void PrintMonthlyStatement(Account account, int year, int month);


        /// <summary>
        /// Prints the entire transaction history for an account.
        /// </summary>
        /// <param name="accountId">The account ID to print the statement for.</param>
        public void PrintStatement(Account account);

    }
}
