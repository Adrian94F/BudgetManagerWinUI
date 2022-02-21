﻿using BudgetManager.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExpBurndownPage : Page, IPageWithInfo
    {
        public string header = "Wypalenie";

        string IPageWithInfo.header { get => header; set => header = value; }

        public ExpBurndownPage()
        {
            this.InitializeComponent();
            Plot();
        }

        private static readonly int nOfDays = (AppData.CurrentMonth.EndDate - AppData.CurrentMonth.StartDate).Days + 2;

        public bool Simplified { get; set; }

        private enum ExpensesSeries
        {
            Burndown,
            BurndownWithoutMonthlyExpenses,
            AverageBurndown,
            AverageBurndownWithoutMonthlyExpenses,
            DailySums,
            MonthlySum
        }
        private enum ExpensesType
        {
            Daily,
            Monthly,
            All
        };

        public IEnumerable<ISeries> Series { get; set; }
        public IEnumerable<ICartesianAxis> XAxis { get; set; }
        public IEnumerable<ICartesianAxis> YAxis { get; set; }
            = new Axis[] {
                new Axis
                {
                    Labeler = value => Math.Round(value).ToString()
                }
            };

        private static string[] GetLabels()
        {
            var labels = new string[nOfDays];
            labels[0] = "";
            for (var i = 1; i < nOfDays; i++)
            {
                labels[i] = AppData.CurrentMonth.StartDate.AddDays(i - 1).ToString("dd");
            }

            return labels;
        }

        private static double?[] GetSumsOfExpenses(ExpensesType type)
        {
            var expenses = new double?[nOfDays];
            expenses[0] = null;
            for (var i = 1; i < nOfDays; i++)
            {
                var date = AppData.CurrentMonth.StartDate.AddDays(i - 1);
                var sum = type == ExpensesType.Daily ? (double)AppData.CurrentMonth.GetSumOfDailyExpensesOfDate(date)
                                                     : (double)AppData.CurrentMonth.GetSumOfMonthlyExpensesOfDate(date);
                sum = Math.Round(sum, 2);
                expenses[i] = sum == 0 ? null : sum;
            }
            return expenses;
        }

        private static StackedColumnSeries<double?> GetColumnSeries(ExpensesSeries series)
        {
            var name = "";
            var values = new double?[] { };
            switch (series)
            {
                case ExpensesSeries.DailySums:
                    name = "Suma codziennych";
                    values = GetSumsOfExpenses(ExpensesType.Daily);
                    break;

                case ExpensesSeries.MonthlySum:
                    name = "Suma miesięcznych";
                    values = GetSumsOfExpenses(ExpensesType.Monthly);
                    break;
            }

            return new StackedColumnSeries<double?>
            {
                Name = name,
                Values = values,
            };
        }

        private static double GetIncomeSum(ExpensesType type)
        {
            var sum = (double)(AppData.CurrentMonth.GetSumOfIncomes());
            switch (type)
            {
                case ExpensesType.Daily:
                    sum -= (double)AppData.CurrentMonth.GetSumOfMonthlyExpenses();
                    break;
                case ExpensesType.All:
                    break;
            }
            return Math.Round(sum, 2);
        }

        private static double[] GetBurndown(ExpensesType type)
        {
            var burnValues = new double[nOfDays];
            var incomeSum = GetIncomeSum(type);

            var yesterdaySum = burnValues[0] = incomeSum;
            for (var i = 1; i < nOfDays; i++)
            {
                burnValues[i] = yesterdaySum;
                switch (type)
                {
                    case ExpensesType.Daily:
                        burnValues[i] -= (double)AppData.CurrentMonth.GetSumOfDailyExpensesOfDate(AppData.CurrentMonth.StartDate.AddDays(i - 1));
                        break;
                    case ExpensesType.All:
                        burnValues[i] -= (double)AppData.CurrentMonth.GetSumOfAllExpensesOfDate(AppData.CurrentMonth.StartDate.AddDays(i - 1));
                        break;
                }
                yesterdaySum = burnValues[i];
                burnValues[i] = Math.Round(burnValues[i], 2);
            }
            return burnValues;
        }

        private static double[] GetAverageBurndown(ExpensesType type)
        {
            var avgBurnValues = new double[nOfDays];
            var incomeSum = GetIncomeSum(type);
            var plannedSavings = (double)AppData.CurrentMonth.PlannedSavings;
            for (var i = 0; i < nOfDays; i++)
            {
                var a = -(incomeSum - plannedSavings) / (nOfDays - 1);
                var b = incomeSum;
                avgBurnValues[i] = Math.Round(a * i + b, 2);
            }
            return avgBurnValues;
        }

        private static LineSeries<double> GetLineSeries(ExpensesSeries series)
        {
            var name = "";
            var values = new double[] { };
            //var stroke = Brushes.Black;
            //var dashArray = new DoubleCollection();
            //var fill = Brushes.Transparent;
            //var pointGeometry = DefaultGeometries.None;
            var lineSmoothness = 0;
            switch (series)
            {
                case ExpensesSeries.Burndown:
                    name = "Wszystkie";
                    values = GetBurndown(ExpensesType.All);
                    //stroke = Brushes.YellowGreen;
                    break;
                case ExpensesSeries.AverageBurndown:
                    name = "Wszystkie (plan)";
                    values = GetAverageBurndown(ExpensesType.All);
                    //stroke = Brushes.YellowGreen;
                    //dashArray = new DoubleCollection { 2 };
                    break;
                case ExpensesSeries.BurndownWithoutMonthlyExpenses:
                    name = "Codzienne";
                    values = GetBurndown(ExpensesType.Daily);
                    //stroke = Brushes.DodgerBlue;
                    break;
                case ExpensesSeries.AverageBurndownWithoutMonthlyExpenses:
                    name = "Codzienne (plan)";
                    values = GetAverageBurndown(ExpensesType.Daily);
                    //stroke = Brushes.DodgerBlue;
                    //dashArray = new DoubleCollection { 2 };
                    break;
            }

            return new LineSeries<double>
            {
                Name = name,
                Values = values,
                //PointGeometry = pointGeometry,
                LineSmoothness = lineSmoothness,
                //Stroke = stroke,
                //StrokeDashArray = dashArray,
                Fill = null
            };
        }

        private static ObservableCollection<ISeries> GetSeries(bool onlyDailyExpenses = false)
        {
            var series = new ObservableCollection<ISeries>();

            series.Add(GetColumnSeries(ExpensesSeries.DailySums));
            series.Add(GetColumnSeries(ExpensesSeries.MonthlySum));

            if (!onlyDailyExpenses)
            {
                series.Add(GetLineSeries(ExpensesSeries.Burndown));
                series.Add(GetLineSeries(ExpensesSeries.AverageBurndown));
            }

            series.Add(GetLineSeries(ExpensesSeries.BurndownWithoutMonthlyExpenses));
            series.Add(GetLineSeries(ExpensesSeries.AverageBurndownWithoutMonthlyExpenses));

            return series;
        }

        private static IEnumerable<ICartesianAxis> GetXAxis(bool simplified)
        {
            return new Axis[] {
                new Axis
                {
                    Labels = GetLabels(),
                    LabelsRotation = 0,
                    ShowSeparatorLines = true,
                    MinStep = 1,
                    ForceStepToMin = !simplified,
                }
            };
        }

        private void Plot()
        {
            Series = GetSeries(Simplified);
            XAxis = GetXAxis(Simplified);
        }
    }
}
