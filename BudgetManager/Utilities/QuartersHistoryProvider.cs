using BudgetManager.Models;
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
    internal class QuartersHistoryProvider
    {
        private static readonly int nOfQuarters = AppData.Quarters.Count;

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
            var labels = new List<String>();
            var quarters = AppData.Quarters;
            foreach (var quarter in quarters)
            {
                labels.Add(quarter.StartDate.Year + "Q" + ((quarter.StartDate.Month + 2) / 3).ToString());
            }
            return labels.ToArray();
        }

        public static ObservableCollection<ISeries> GetSeries()
        {
            // incomes, expenses and balances
            var monthIncomes = new double[nOfQuarters];
            var monthExpenses = new double[nOfQuarters];
            var monthBalances = new double[nOfQuarters];
            var quarters = AppData.Quarters;
            for (var i = 0; i < nOfQuarters; i++)
            {
                monthIncomes[i] = (double)quarters.ElementAt(i).GetSumOfIncomes();
                monthExpenses[i] = (double)quarters.ElementAt(i).GetSumOfExpenses();
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
