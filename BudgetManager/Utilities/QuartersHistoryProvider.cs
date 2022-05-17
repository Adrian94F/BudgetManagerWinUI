using BudgetManager.Models;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
                    GeometrySize = 0,
                    GeometryStroke = null,
                    GeometryFill = null,
                    Stroke = new SolidColorPaint
                    {
                        Color = new SKColor(154, 205, 50),
                        StrokeThickness = 1.5f
                    },
                    Fill = null,
                    Name = "Suma przychodów"
                },
                new LineSeries<double>
                {
                    Values = monthExpenses,
                    GeometrySize = 0,
                    GeometryStroke = null,
                    GeometryFill = null,
                    Stroke = new SolidColorPaint
                    {
                        Color = new SKColor(255, 0, 0),
                        StrokeThickness = 1.5f
                    },
                    Fill = null,
                    Name = "Suma wydatków"
                },
                new ColumnSeries<double>
                {
                    Values = monthBalances,
                    Fill = new SolidColorPaint(new SKColor(255, 215, 0)),
                    Stroke = new SolidColorPaint
                    {
                        Color = new SKColor(230, 194, 0),
                        StrokeThickness = 2.0f,
                    },
                    Name = "Bilans",
                }
            };
            return series;
        }
    }
}
