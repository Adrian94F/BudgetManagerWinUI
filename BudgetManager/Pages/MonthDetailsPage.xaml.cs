using BudgetManager.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonthDetailsPage : Page, IPageWithInfo
    {
        public string header = "Szczegóły miesiąca";

        string IPageWithInfo.header { get => header; set => header = value; }

        public MonthDetailsPage()
        {
            this.InitializeComponent();

            FillPage();
        }

        private void FillPage()
        {
            var month = AppData.SelectedMonth;
            StartDatePicker.Date = month.StartDate;
            EndDatePicker.Date = month.EndDate;
            if (month.IsCurrent())
            {
                PlannedSavingsNumberBox.Value = decimal.ToDouble(month.PlannedSavings);
            }
            else
            {
                PlanedSavingsStackPanel.Visibility = Visibility.Collapsed;
            }
            if (month.Expenses.Count == 0)
            {
                RemoveMonthButton.IsEnabled = true;
            }
            FillIncomesTable();
        }

        private void FillIncomesTable()
        {
            var incomesCollection = new ObservableCollection<Income>(AppData.SelectedMonth.Incomes);
            IncomesDataGrid.ItemsSource = incomesCollection;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var income = (sender as FrameworkElement).DataContext as Income;
            AppData.SelectedMonth.Incomes.Remove(income);
            (IncomesDataGrid.ItemsSource as ObservableCollection<Income>).Remove(income);
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            var inc = new Income();
            AppData.SelectedMonth.Incomes.Add(inc);
            FillIncomesTable();
        }

        private void DatePicker_DateChanged(object sender, CalendarDatePickerDateChangedEventArgs e)
        {
            var picker = sender as CalendarDatePicker;
            var date = picker.Date;
            switch (picker.Name)
            {
                case "StartDatePicker":
                    if (date <= AppData.SelectedMonth.EndDate)
                    {
                        AppData.SelectedMonth.StartDate = date.Value.Date;
                        return;
                    }
                    break;
                case "EndDatePicker":
                    if (date >= AppData.SelectedMonth.StartDate)
                    {
                        AppData.SelectedMonth.EndDate = date.Value.Date;
                        return;
                    }
                    break;
            }
        }

        private void PlannedSavingsNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var newValue = args.NewValue;
            if (double.IsNaN(newValue))
            {
                newValue = 0;
            }
            AppData.SelectedMonth.PlannedSavings = new decimal(newValue);
        }

        private void RemoveMonthButton_Click(object sender, RoutedEventArgs e)
        {
            var current = AppData.SelectedMonth;
            AppData.Months.Remove(current);
            AppData.SelectedMonth = AppData.Months.LastOrDefault();

            App.m_window.GoHome();
        }
    }
}
