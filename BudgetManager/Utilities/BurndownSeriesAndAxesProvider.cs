using BudgetManager.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Utilities
{
    internal static class BurndownSeriesAndAxesProvider
    {
        private static readonly int nOfDays = (AppData.CurrentMonth.EndDate - AppData.CurrentMonth.StartDate).Days + 2;

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
                expenses[i] = Math.Round(sum, 2) == 0 ? null : Math.Round(sum, 0);
            }
            return expenses;
        }

        private static StackedColumnSeries<double?> GetColumnSeries(ExpensesSeries series)
        {
            var name = "";
            var values = new double?[] { };
            var fillColor = new SKColor(180, 180, 180);
            var strokeColor = new SKColor(100, 100, 100);
            var strokeThickness = 2.0f;
            switch (series)
            {
                case ExpensesSeries.DailySums:
                    name = "Suma codziennych";
                    values = GetSumsOfExpenses(ExpensesType.Daily);
                    fillColor = new SKColor(255, 215, 0);
                    strokeColor = new SKColor(230, 194, 0);
                    break;

                case ExpensesSeries.MonthlySum:
                    name = "Suma miesięcznych";
                    values = GetSumsOfExpenses(ExpensesType.Monthly);
                    fillColor = new SKColor(230, 194, 0);
                    strokeColor = new SKColor(204, 172, 0);
                    break;
            }

            return new StackedColumnSeries<double?>
            {
                Name = name,
                Values = values,
                Fill = new SolidColorPaint(fillColor),
                Stroke = new SolidColorPaint
                {
                    Color = strokeColor,
                    StrokeThickness = strokeThickness,
                },
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
                burnValues[i] = Math.Round(burnValues[i], 0);
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
                avgBurnValues[i] = Math.Round(a * i + b, 0);
            }
            return avgBurnValues;
        }

        private static LineSeries<double> GetLineSeries(ExpensesSeries series)
        {
            var name = "";
            var values = new double[] { };
            var color = new SKColor(180, 180, 180);
            var strokeThickness = 2.0f;
            var strokeDashArray = new float[] { 3 * strokeThickness, 2 * strokeThickness };
            var effect = new DashEffect(new float[] { });
            switch (series)
            {
                case ExpensesSeries.Burndown:
                    name = "Wszystkie";
                    values = GetBurndown(ExpensesType.All);
                    color = new SKColor(154, 205, 50);
                    break;
                case ExpensesSeries.BurndownWithoutMonthlyExpenses:
                    name = "Codzienne";
                    values = GetBurndown(ExpensesType.Daily);
                    color = new SKColor(30, 144, 255);
                    break;
                case ExpensesSeries.AverageBurndown:
                    name = "Wszystkie (plan)";
                    values = GetAverageBurndown(ExpensesType.All);
                    effect = new DashEffect(strokeDashArray);
                    strokeThickness = 1.5f;
                    color = new SKColor(154, 205, 50);
                    break;
                case ExpensesSeries.AverageBurndownWithoutMonthlyExpenses:
                    name = "Codzienne (plan)";
                    values = GetAverageBurndown(ExpensesType.Daily);
                    effect = new DashEffect(strokeDashArray);
                    strokeThickness = 1.5f;
                    color = new SKColor(30, 144, 255);
                    break;
            }

            return new LineSeries<double>
            {
                Name = name,
                Values = values,
                GeometrySize = 0,
                GeometryStroke = null,
                GeometryFill = null,
                Stroke = new SolidColorPaint
                {
                    Color = color,
                    StrokeThickness = strokeThickness,
                    PathEffect = effect,
                },
                Fill = null
            };
        }

        public static ObservableCollection<ISeries> GetSeries(bool simplified = false)
        {
            var series = new ObservableCollection<ISeries>();

            series.Add(GetColumnSeries(ExpensesSeries.DailySums));
            series.Add(GetColumnSeries(ExpensesSeries.MonthlySum));

            if (!simplified)
            {
                series.Add(GetLineSeries(ExpensesSeries.Burndown));
                series.Add(GetLineSeries(ExpensesSeries.AverageBurndown));
            }

            series.Add(GetLineSeries(ExpensesSeries.BurndownWithoutMonthlyExpenses));
            series.Add(GetLineSeries(ExpensesSeries.AverageBurndownWithoutMonthlyExpenses));

            return series;
        }

        public static IEnumerable<ICartesianAxis> GetXAxes(bool simplified)
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

        public static IEnumerable<ICartesianAxis> GetYAxes()
        {
            return new Axis[] {
                new Axis
                {
                    Labeler = value => Math.Round(value).ToString()
                }
            };
        }
    }
}
