﻿using BudgetManager.Models;
using BudgetManager.Utilities;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
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
    public sealed partial class QuartersHistoryPage : Page, IPageWithInfo
    {
        public string header = "Historia kwartałów";

        private static readonly int WidthOfQuarter = 100;

        string IPageWithInfo.header { get => header; set => header = value; }

        public QuartersHistoryPage()
        {
            this.InitializeComponent();
            ChangeGridWidth();
        }

        public IEnumerable<ISeries> Series { get; set; } = HistoryProvider.GetSeries();
        public IEnumerable<ICartesianAxis> XAxis { get; set; } = HistoryProvider.GetXAxes();
        public IEnumerable<ICartesianAxis> YAxis { get; set; } = HistoryProvider.GetYAxes();

        private void ChangeGridWidth()
        {
            HistoryChartGrid.Width = WidthOfQuarter * AppData.Months.Count - 1;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryScrollViewer.ScrollToHorizontalOffset(HistoryScrollViewer.ScrollableWidth);
        }
    }
}
