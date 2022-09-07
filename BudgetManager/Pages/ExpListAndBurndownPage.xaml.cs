using BudgetManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

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

            if (AppData.SelectedMonth != null)
            {
                var month = AppData.SelectedMonth;
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
