using BudgetManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
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
    public sealed partial class ExpListAndBurndownPage : Page, IPageWithInfo
    {
        public string header = "Lista wydatków i wypalenie";

        string IPageWithInfo.header { get => header; set => header = value; }

        public ExpListAndBurndownPage()
        {
            this.InitializeComponent();

            if (AppData.CurrentMonth != null)
            {
                var month = AppData.CurrentMonth;
                var startDate = month.StartDate.ToString("dd.MM.yyyy");
                var endDate = month.EndDate.ToString("dd.MM.yyyy");
                header += " " + startDate + "-" + endDate;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            (ListFrame.Content as ExpListPage).ExpChanged += ExpListAndBurndownPage_ExpChanged;
        }

        private void ExpListAndBurndownPage_ExpChanged(object sender, EventArgs e)
        {
            // refresh burndown
            (BurndownFrame.Content as ExpBurndownPage).Plot();
        }
    }
}
