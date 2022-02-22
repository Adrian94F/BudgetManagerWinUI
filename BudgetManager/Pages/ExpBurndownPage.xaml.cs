using BudgetManager.Models;
using BudgetManager.Utilities;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SkiaSharp;
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

        public bool Simplified { get; set; }

        public IEnumerable<ISeries> Series { get; set; }
        public IEnumerable<ICartesianAxis> XAxes { get; set; }
        public IEnumerable<ICartesianAxis> YAxes { get; set; }

        private void Plot()
        {
            Series = BurndownSeriesAndAxesProvider.GetSeries(Simplified);
            XAxes = BurndownSeriesAndAxesProvider.GetXAxes(Simplified);
            YAxes = BurndownSeriesAndAxesProvider.GetYAxes();
        }
    }
}
