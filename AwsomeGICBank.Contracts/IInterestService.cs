using AwsomeGICBank.Models;
using System;
using System.Collections.Generic;

namespace AwsomeGICBank.Contracts
{
    public interface IInterestService
    {
        /// <summary>
        /// Prompts the user to define new interest rules via the console.
        /// </summary>
        void DefineInterestRule();

        /// <summary>
        /// Adds or updates an interest rule for a specific date.
        /// </summary>
        /// <param name="date">The date the rule applies from.</param>
        /// <param name="ruleId">The ID of the rule.</param>
        /// <param name="rate">The interest rate for this rule.</param>
        void AddOrUpdateInterestRule(DateTime date, string ruleId, decimal rate);

        /// <summary>
        /// Gets the applicable interest rule for a specific date.
        /// </summary>
        /// <param name="date">The date for which to get the applicable interest rule.</param>
        /// <returns>The applicable interest rule.</returns>
        InterestRule GetApplicableRule(DateTime date);

        /// <summary>
        /// Calculates the monthly interest for a specified account and month.
        /// </summary>
        /// <param name="account">The account to calculate interest for.</param>
        /// <param name="year">The year of the calculation.</param>
        /// <param name="month">The month of the calculation.</param>
        /// <returns>The calculated interest for the month.</returns>
        decimal CalculateMonthlyInterest(Account account, int year, int month);

        /// <summary>
        /// Gets the starting balance of an account at the beginning of a specified month.
        /// </summary>
        /// <param name="account">The account to calculate the starting balance for.</param>
        /// <param name="year">The year of the calculation.</param>
        /// <param name="month">The month of the calculation.</param>
        /// <returns>The starting balance for the specified month.</returns>
        decimal GetStartingBalance(Account account, int year, int month);

        /// <summary>
        /// Prints the defined interest rules to the console.
        /// </summary>
        void PrintInterestRules();
    }
}