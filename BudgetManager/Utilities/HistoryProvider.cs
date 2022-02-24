﻿using BudgetManager.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Utilities
{
    internal class HistoryProvider
    {
        private static readonly int nOfMonths = AppData.months.Count - 1;

        public static IEnumerable<ICartesianAxis> GetXAxes()
        {
            return new Axis[] {
                new Axis
                {
                    Labels = GetLabels(),
                    LabelsRotation = 0,
                }
            };
        }

        public static IEnumerable<ICartesianAxis> GetYAxes()
        {
            return new Axis[] {
                new Axis
                {
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                    Labeler = value => Math.Round(value).ToString()
                }
            };
        }

        private static string[] GetLabels()
        {
            var labels = new string[nOfMonths];
            for (var i = 0; i < nOfMonths; i++)
            {
                labels[i] = AppData.months.ElementAt(i).StartDate.ToString("dd.MM") + "-" + AppData.months.ElementAt(i).EndDate.ToString("dd.MM");
            }
            return labels;
        }

        public static ObservableCollection<ISeries> GetSeries()
        {
            // incomes, expenses and balances
            var monthIncomes = new double[nOfMonths];
            var monthExpenses = new double[nOfMonths];
            var monthBalances = new double[nOfMonths];
            for (var i = 0; i < nOfMonths; i++)
            {
                monthIncomes[i] = (double)(AppData.months.ElementAt(i).GetSumOfIncomes());
                monthExpenses[i] = (double)AppData.months.ElementAt(i).GetSumOfExpenses();
                monthBalances[i] = Math.Round(monthIncomes[i] - monthExpenses[i], 2);
            }

            var series = new ObservableCollection<ISeries>
            {
                new LineSeries<double>
                {
                    Values = monthIncomes,
                    Fill = null,
                    Name = "Suma przychodów"
                },
                new LineSeries<double>
                {
                    Values = monthExpenses,
                    Fill = null,
                    Name = "Suma wydatków"
                },
                new ColumnSeries<double>
                {
                    Values = monthBalances,
                    Name = "Bilans",
                }
            };
            return series;
        }
    }
}
