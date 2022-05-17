using BudgetManager.Models;
using Microsoft.UI.Xaml.Controls;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummaryPage : Page, IPageWithInfo
    {
        public string header = "Podsumowanie";

        string IPageWithInfo.header { get => header; set => header = value; }

        public SummaryPage()
        {
            this.InitializeComponent();
            FillPage();
        }

        public void FillPage()
        {
            if (AppData.CurrentMonth != null)
            {
                var month = AppData.CurrentMonth;
                FillSummaryGrid(month);
            }
        }

        private void FillSummaryGrid(Month month)
        {
            var startDate = month.StartDate.ToString("dd.MM.yyyy");
            var endDate = month.EndDate.ToString("dd.MM.yyyy");
            var net = month.GetSumOfIncomes(Income.IncomeType.Salary);
            var incSum = month.GetSumOfIncomes();
            var add = incSum - net;
            var expSum = month.GetSumOfExpenses();
            var monthlyExpSum = month.GetSumOfMonthlyExpenses();
            var dailyExpSum = expSum - monthlyExpSum;
            var savings = month.PlannedSavings;
            var balance = incSum - expSum - savings;
            var isActualMonth = (DateTime.Today - month.StartDate).Days >= 0 && (month.EndDate - DateTime.Today).Days >= 0;
            var daysLeft = (month.EndDate - DateTime.Today).Days + 1;
            var balanceWithoutToday = balance + month.GetSumOfAllExpensesOfDate(DateTime.Today);
            var estimatedExpense = balanceWithoutToday >= 0 ? (isActualMonth ? Math.Round(balanceWithoutToday / daysLeft, 2) : Math.Round(balance, 2)) : 0;
            var todayExpenses = month.GetSumOfDailyExpensesOfDate(DateTime.Today);
            var todayExpensesPercent = estimatedExpense != 0 ? todayExpenses / estimatedExpense * 100 : -1;

            header += " " + startDate + "-" + endDate;
            NetIncomeTextBlock.Text = net.ToString("F") + " zł";
            AddIncomeTextBlock.Text = add.ToString("F") + " zł";
            IncomeSumTextBlock.Text = incSum.ToString("F") + " zł";
            IncomeSumTextBlock2.Text = incSum.ToString("F") + " zł";
            DailyExpensesSumTextBlock.Text = dailyExpSum.ToString("F") + " zł";
            MonthlyExpensesSumTextBlock.Text = monthlyExpSum.ToString("F") + " zł";
            ExpensesSumTextBlock.Text = expSum.ToString("F") + " zł";
            ExpensesSumTextBlock2.Text = expSum.ToString("F") + " zł";
            PlannedSavingsTextBlock.Text = savings.ToString("F") + " zł";
            BalanceTextBlock.Text = (balance > 0 ? "+" : "") + balance.ToString("F") + " zł";
            DaysLeftTextBlock.Text = isActualMonth ? daysLeft.ToString() : "-";
            EstimatedDailyExpenseTextBlock.Text = isActualMonth ? estimatedExpense.ToString("F") + " zł" : "-";
            SumOfTodayExpenses.Text = isActualMonth ? todayExpenses.ToString("F") + " zł" : "-";
            PercentOfTodayExpenses.Text = todayExpensesPercent >= 0 ? (isActualMonth ? todayExpensesPercent.ToString("F0") + "%" : "-") : "-";
        }
    }
}
