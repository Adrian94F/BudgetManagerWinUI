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

        public event EventHandler ExpChanged;

        private Month month_ = AppData.CurrentMonth;
        private DateTime? date_ = null;
        private Category category_ = null;

        string IPageWithInfo.header { get => header; set => header = value; }

        public ExpListPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                if (e.Parameter is DateTime date)
                {
                    date_ = date;
                }
                else if (e.Parameter is Category category)
                {
                    category_ = category;  // AppData.categories.Where( cat => cat.Name.Equals(category) ).First();
                }
                else if (e.Parameter is Tuple<DateTime, Category> dateAndCategory)
                {
                    date_ = dateAndCategory.Item1;
                    category_ = dateAndCategory.Item2;
                }

                if (date_ != null)
                {
                    header += " w dniu " + date_.Value.ToString("dd.MM.yyyy");
                }
                if (category_ != null)
                {
                    header += " z kategorii " + category_.Name;
                }
            }
            FillWithExpenses();
        }

        private void FillWithExpenses()
        {
            var expenses = new SortedSet<Expense>(date_ == null
                ? category_ == null
                    ? month_.Expenses
                    : month_.GetExpensesOfCategory(category_)
                : month_.GetExpensesOfCategoryAndDate(category_, date_.Value));
            var lastDay = "";

            ExpListListView.Items.Clear();
            if (expenses.Count == 0)
            {
                var item = new TextBlock()
                {
                    Text = "brak wydatków",
                    FontStyle = Windows.UI.Text.FontStyle.Italic
                };
                ExpListListView.Items.Add(item);
            }
            else
            {
                foreach (var exp in expenses.Reverse())
                {
                    var day = exp.Date.ToString("d");
                    if (lastDay != day)
                    {
                        lastDay = day;
                        var item = new ListViewHeaderItem()
                        {
                            Content = day,
                        };
                        ExpListListView.Items.Add(item);
                    }

                    var expItem = new ExpenseListViewItem()
                    {
                        OriginalExpense = exp,
                    };

                    ExpListListView.Items.Add(expItem);
                }
            }
        }

        private void ExpListListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is ExpenseListViewItem exp && !ExpListSplitView.IsPaneOpen)
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

            ExpChanged?.Invoke(this, args);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Logger.Log("add expense clicked");
            ExpListSplitView.IsPaneOpen = true;
            var detailsControl = date_ == null
                ? new ExpDetails(category_)
                : category_ == null
                    ? new ExpDetails(date_.Value)
                    : new ExpDetails(date_.Value, category_);
            detailsControl.ExpenseChanged += new EventHandler(ExpDetails_ExpChanged);
            ExpListSplitViewPane.Child = detailsControl;
        }
    }
}
