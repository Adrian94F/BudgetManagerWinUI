using BudgetManager.Controls;
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
    public sealed partial class ExpListPage : Page, IPageWithInfo
    {
        public string header = "Lista wydatków";

        string IPageWithInfo.header { get => header; set => header = value; }

        public ExpListPage()
        {
            this.InitializeComponent();
            FillWithExpenses();
        }

        private void FillWithExpenses()
        {
            var lastDay = "";
            var month = AppData.CurrentMonth;
            if (month == null)
            {
                return;
            }

            var expenses = new SortedSet<Expense>(month.Expenses);

            ExpListListView.Items.Clear();
            foreach (var exp in expenses.Reverse())
            {
                var day = exp.Date.ToString("d");
                if (lastDay != day)
                {
                    lastDay = day;
                    var text = new TextBlock()
                    {
                        Text = day,
                        FontWeight = FontWeights.Bold
                    };
                    ExpListListView.Items.Add(text);
                }

                var expItem = new ExpenseListViewItem()
                {
                    OriginalExpense = exp,
                };

                ExpListListView.Items.Add(expItem);
            }
        }

        private void ExpListListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var exp = e.ClickedItem as ExpenseListViewItem;
            if (exp != null && !ExpListSplitView.IsPaneOpen)
            {
                Logger.Log("clicked expense: " + exp.OriginalExpense.ToString());
                
                ExpListSplitView.IsPaneOpen = true;
                var detailsControl = new ExpDetails()
                {
                    Expense = exp.OriginalExpense
                };
                detailsControl.ExpenseChanged += new EventHandler(ExpDetails_ExpChanged);
                ExpListSplitViewPane.Child = detailsControl;
            }
            else
            {
                ExpListSplitView.IsPaneOpen = false;
            }
        }

        private void ExpDetails_ExpChanged(object sender, EventArgs args)
        {
            FillWithExpenses();
            var details = sender as ExpDetails;
            if (details != null)
            {
                if (!AppData.CurrentMonth.Expenses.Contains(details.Expense))
                {
                    ExpListSplitView.IsPaneOpen = false;
                }
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("add expense clicked");
            ExpListSplitView.IsPaneOpen = true;
            var detailsControl = new ExpDetails();
            detailsControl.ExpenseChanged += new EventHandler(ExpDetails_ExpChanged);
            ExpListSplitViewPane.Child = detailsControl;
        }
    }
}
