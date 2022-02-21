using BudgetManager.Models;
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
    public sealed partial class MonthsHistoryPage : Page, IPageWithInfo
    {
        public string header = "Historia";

        string IPageWithInfo.header { get => header; set => header = value; }

        private static readonly int WidthOfMonth = 100;
        private static readonly int nOfMonths = AppData.months.Count - 1;  // omit last (current) month

        public IEnumerable<ISeries> Series { get; set; }
        public IEnumerable<ICartesianAxis> XAxis { get; set; }
            = new Axis[] {
                new Axis
                {
                    Labels = GetLabels(),
                    LabelsRotation = 0,
                }
            };
        public IEnumerable<ICartesianAxis> YAxis { get; set; }
            = new Axis[] {
                new Axis
                {
                    Position = LiveChartsCore.Measure.AxisPosition.End,
                    Labeler = value => Math.Round(value).ToString()
                }
            };

        public MonthsHistoryPage()
        {
            this.InitializeComponent();
            ChangeGridWidth();
            Plot();
        }

        private void ChangeGridWidth()
        {
            HistoryChartGrid.Width = WidthOfMonth * AppData.months.Count - 1;
        }

        private void Plot()
        {
            Series = GetSeries();
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

        private static ObservableCollection<ISeries> GetSeries()
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryScrollViewer.ScrollToHorizontalOffset(HistoryScrollViewer.ScrollableWidth);
        }
    }
}
