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

        public MonthsListPage()
        {
            this.InitializeComponent();
            FillWithMonths();
        }

        private void FillWithMonths()
        {
            var lastYear = 0;
            foreach (var month in AppData.months.Reverse())
            {
                var startYear = month.startDate.Year;
                if (startYear != lastYear)
                {
                    lastYear = startYear;
                    var text = new TextBlock()
                    {
                        Text = startYear.ToString(),
                        FontWeight = FontWeights.Bold
                    };
                    var item = new ListViewItem()
                    {
                        Padding = new Thickness(20, 0, 0, 0),
                        Content = text
                    };
                    MonthsListView.Items.Add(item);
                }
                MonthsListView.Items.Add(month);
            }
        }


        private void MonthsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var month = e.ClickedItem as Month;
            if (month != null)
            {
                Logger.Log("clicked month: " + month.ToString());
            }
        }
    }
}
