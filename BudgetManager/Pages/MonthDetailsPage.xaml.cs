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
            var month = AppData.CurrentMonth;
            StartDatePicker.SelectedDate = month.StartDate;
            EndDatePicker.SelectedDate = month.EndDate;
            PlannedSavingsNumberBox.Value = decimal.ToDouble(month.PlannedSavings);
            if (month.Expenses.Count == 0)
            {
                RemoveMonthButton.IsEnabled = true;
            }
            FillIncomesTable();
        }

        private void FillIncomesTable()
        {
            var incomesCollection = new ObservableCollection<Income>(AppData.CurrentMonth.Incomes);
            IncomesDataGrid.ItemsSource = incomesCollection;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var income = (sender as FrameworkElement).DataContext as Income;
            AppData.CurrentMonth.Incomes.Remove(income);
            (IncomesDataGrid.ItemsSource as ObservableCollection<Income>).Remove(income);
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            var inc = new Income();
            AppData.CurrentMonth.Incomes.Add(inc);
            FillIncomesTable();
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            var date = picker.SelectedDate;
            switch (picker.Name)
            {
                case "StartDatePicker":
                    if (date <= AppData.CurrentMonth.EndDate)
                    {
                        AppData.CurrentMonth.StartDate = picker.SelectedDate.Value.Date;
                        return;
                    }
                    break;
                case "EndDatePicker":
                    if (date >= AppData.CurrentMonth.StartDate)
                    {
                        AppData.CurrentMonth.EndDate = picker.SelectedDate.Value.Date;
                        return;
                    }
                    break;
            }
            var oldDate = e.OldDate;
            picker.SelectedDate = oldDate;
            picker.Date = oldDate;
        }

        private void PlannedSavingsNumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var newValue = args.NewValue;
            if (double.IsNaN(newValue))
            {
                newValue = 0;
            }
            AppData.CurrentMonth.PlannedSavings = new decimal(newValue);
        }

        private void RemoveMonthButton_Click(object sender, RoutedEventArgs e)
        {
            var current = AppData.CurrentMonth;
            AppData.Months.Remove(current);
            AppData.CurrentMonth = AppData.Months.LastOrDefault();

            App.m_window.GoHome();
        }
    }
}
