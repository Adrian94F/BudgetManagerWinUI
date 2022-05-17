using BudgetManager.Models;
using BudgetManager.Utilities;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonthsHistoryPage : Page, IPageWithInfo
    {
        public string header = "Historia miesięczna";

        string IPageWithInfo.header { get => header; set => header = value; }

        private static readonly int WidthOfMonth = 120;

        public IEnumerable<ISeries> Series { get; set; } = MonthsHistoryProvider.GetSeries();
        public IEnumerable<ICartesianAxis> XAxis { get; set; } = MonthsHistoryProvider.GetXAxes();
        public IEnumerable<ICartesianAxis> YAxis { get; set; } = MonthsHistoryProvider.GetYAxes();

        public MonthsHistoryPage()
        {
            this.InitializeComponent();
            ChangeGridWidth();
        }

        private void ChangeGridWidth()
        {
            HistoryChartGrid.MinWidth = WidthOfMonth * AppData.Months.Count - 1;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryScrollViewer.ScrollToHorizontalOffset(HistoryScrollViewer.ScrollableWidth);
        }
    }
}
