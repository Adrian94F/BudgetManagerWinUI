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
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Utilities
{
    internal class MonthsHistoryProvider
    {
        private static readonly int nOfMonths = AppData.Months.Count - 1;

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
                labels[i] = AppData.Months.ElementAt(i).StartDate.ToString("dd.MM") + "-" + AppData.Months.ElementAt(i).EndDate.ToString("dd.MM");
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
                monthIncomes[i] = (double)(AppData.Months.ElementAt(i).GetSumOfIncomes());
                monthExpenses[i] = (double)AppData.Months.ElementAt(i).GetSumOfExpenses();
                monthBalances[i] = Math.Round(monthIncomes[i] - monthExpenses[i], 2);
            }

            var color = new SKColor(180, 180, 180);
            var strokeThickness = 2.0f;
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
