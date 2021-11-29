using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using BudgetManager.Models;

namespace BudgetManager
{
    public class Month : IComparable
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal PlannedSavings { get; set; }

        public HashSet<Expense> Expenses { get; set; }

        public HashSet<Income> Incomes { get; set; }

        public Month()
        {
            Incomes = new HashSet<Income>();
            Expenses = new HashSet<Expense>();
        }

        public override string ToString()
        {
            var start = StartDate.ToString("dd.MM.yyyy");
            var end = EndDate.ToString("dd.MM.yyyy");
            return start + "-" + end;
        }

        public decimal GetSumOfIncomes(Income.IncomeType? type = null)
        {
            var sum = decimal.Zero;
            foreach (var income in Incomes)
                if (type == null || income.Type == type)
                    sum += income.Value;
            return sum;
        }

        public HashSet<Expense> GetCopyOfMonthlyExpensesForNextPeriod()
        {
            var ret = new HashSet<Expense>();
            foreach (var exp in Expenses)
            {
                if (exp.MonthlyExpense)
                {
                    var newExp = new Expense()
                    {
                        Value = exp.Value,
                        Date = exp.Date.AddMonths(1),
                        Comment = exp.Comment,
                        Category = exp.Category,
                        MonthlyExpense = true
                    };
                    ret.Add(newExp);
                }
            }
            return ret;
        }

        public bool IsEmpty()
        {
            return Expenses.Count == 0;
        }

        public int CompareTo(object obj)
        {
            return StartDate.CompareTo(((Month)obj).StartDate);
        }

        public decimal GetSumOfExpensesOfCategoryAndDate(Category category, DateTime date)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;

            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    var expDay = expense.Date.Day;
                    var expMonth = expense.Date.Month;
                    if (expense.Category == category && day == expDay && month == expMonth)
                    {
                        ret += expense.Value;
                    }
                }
            }
            return ret;
        }

        public HashSet<Expense> GetExpensesOfCategoryAndDate(Category category, DateTime date)
        {
            var ret = new HashSet<Expense>();
            var day = date.Day;
            var month = date.Month;
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    var expDay = expense.Date.Day;
                    var expMonth = expense.Date.Month;

                    if (day == expDay && month == expMonth)
                    {
                        if (category == null)
                        {
                            ret.Add(expense);
                        }
                        else if (expense.Category == category)
                        {
                            ret.Add(expense);
                        }
                    }
                }
            }
            return ret;
        }

        public HashSet<Expense> GetExpensesOfCategory(Category category)
        {
            var ret = new HashSet<Expense>();
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    if (category == null)
                    {
                        ret.Add(expense);
                    }
                    else if (expense.Category == category)
                    {
                        ret.Add(expense);
                    }
                }
            }
            return ret;
        }

        private decimal GetSumOfValidExpensesOfDate(DateTime date, Func<Expense, bool> validator)
        {
            var ret = Decimal.Zero;
            var day = date.Day;
            var month = date.Month;
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    var expDay = expense.Date.Day;
                    var expMonth = expense.Date.Month;
                    if (day == expDay && month == expMonth)
                    {
                        if (validator(expense))
                        {
                            ret += expense.Value;
                        }
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfAllExpensesOfDate(DateTime date)
        {
            return GetSumOfValidExpensesOfDate(date, exp => true);
        }

        public decimal GetSumOfDailyExpensesOfDate(DateTime date)
        {
            return GetSumOfValidExpensesOfDate(date, exp => !exp.MonthlyExpense);
        }

        public decimal GetSumOfMonthlyExpensesOfDate(DateTime date)
        {
            return GetSumOfAllExpensesOfDate(date) - GetSumOfDailyExpensesOfDate(date);
        }

        public decimal GetSumOfMonthlyExpenses()
        {
            var ret = Decimal.Zero;
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    if (expense.MonthlyExpense)
                    {
                        ret += expense.Value;
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfExpensesOfCategory(Category category)
        {
            var ret = Decimal.Zero;
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    if (expense.Category == category)
                    {
                        ret += expense.Value;
                    }
                }
            }
            return ret;
        }

        public decimal GetSumOfExpenses()
        {
            var ret = Decimal.Zero;
            if (Expenses != null)
            {
                foreach (var expense in Expenses)
                {
                    ret += expense.Value;
                }
            }
            return ret;
        }
    }
}
