﻿using BudgetManager.Models;
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
    public sealed partial class ExpBurndownPage : Page, IPageWithInfo
    {
        public string header = "Wypalenie";

        string IPageWithInfo.header { get => header; set => header = value; }

        public ExpBurndownPage()
        {
            this.InitializeComponent();

            if (AppData.SelectedMonth != null)
            {
                var month = AppData.SelectedMonth;
                var startDate = month.StartDate.ToString("dd.MM.yyyy");
                var endDate = month.EndDate.ToString("dd.MM.yyyy");
                header += " " + startDate + "-" + endDate;
            }
        }

        public bool Simplified { get; set; }

        public IEnumerable<ISeries> Series { get; set; }
        public IEnumerable<ICartesianAxis> XAxes { get; set; }
        public IEnumerable<ICartesianAxis> YAxes { get; set; }

        public void Plot()
        {
            BurndownChart.Series = BurndownProvider.GetSeries(Simplified);
            BurndownChart.XAxes = BurndownProvider.GetXAxes(Simplified);
            BurndownChart.YAxes = BurndownProvider.GetYAxes();
            if (!Simplified)
            {
                BurndownChart.LegendPosition = LiveChartsCore.Measure.LegendPosition.Bottom;
            }
            BurndownChart.Sections = BurndownProvider.GetSections();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Plot();
        }
    }
}
