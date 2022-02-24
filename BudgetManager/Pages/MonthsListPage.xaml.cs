using BudgetManager.Models;
using BudgetManager.Utilities;
using Microsoft.UI.Text;
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
    public sealed partial class MonthsListPage : Page, IPageWithInfo
    {
        public string header = "Lista miesięcy";

        string IPageWithInfo.header { get => header; set => header = value; }

        public EventHandler MonthSelected { get; set; }

        public MonthsListPage()
        {
            this.InitializeComponent();
            FillWithMonths();
        }

        private void FillWithMonths()
        {
            var lastYear = 0;
            foreach (var month in AppData.Months.Reverse())
            {
                var startYear = month.StartDate.Year;
                if (startYear != lastYear)
                {
                    lastYear = startYear;
                    var item = new ListViewHeaderItem()
                    {
                        Padding = new Thickness(20, 0, 0, 0),
                        Content = startYear,
                    };
                    MonthsListView.Items.Add(item);
                }
                MonthsListView.Items.Add(month);
                if (AppData.CurrentMonth == month)
                {
                    MonthsListView.SelectedItem = month;
                }
            }
        }

        private void MonthsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Month month)
            {
                Logger.Log("clicked month: " + month.ToString());
                AppData.CurrentMonth = month;
                MonthSelected?.Invoke(this, new EventArgs());
            }
        }

        private void MonthsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.First() is not Month)
            {
                MonthsListView.SelectedItem = AppData.CurrentMonth;
            }
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            // create new month
            // select new month in AppData.CurrentMonth
        }
    }
}
