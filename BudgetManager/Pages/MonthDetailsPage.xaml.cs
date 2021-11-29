﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using BudgetManager.Models;
using System.Collections.ObjectModel;
using CommunityToolkit.WinUI.UI.Controls;

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
            var month = AppData.CurrentMonth;
            var incomesCollection = new ObservableCollection<IncomeDataItem>();
            foreach (var inc in month.Incomes)
            {
                incomesCollection.Add(new IncomeDataItem(inc));
            }
            IncomesDataGrid.ItemsSource = incomesCollection;
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext as IncomeDataItem;
            var income = item.originalIncome as Income;
            AppData.CurrentMonth.Incomes.Remove(income);
            (IncomesDataGrid.ItemsSource as ObservableCollection<IncomeDataItem>).Remove(item);
        }

        private void IncomesDataGrid_RowEditEnded(object sender, DataGridRowEditEndedEventArgs e)
        {
            var item = e.Row.DataContext as IncomeDataItem;
            if (item != null)
            {
                decimal value;
                var parsed = decimal.TryParse(item.value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.CurrentCulture, out value);
                if (parsed)
                {
                    var income = item.originalIncome;
                    income.value = value;
                    income.type = item.isSalary ? Income.IncomeType.Salary : Income.IncomeType.Additional;
                    income.comment = item.comment;
                }
                FillIncomesTable();
            }
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
            AppData.months.Remove(current);
            AppData.CurrentMonth = AppData.months.LastOrDefault();
        }
    }

    class IncomeDataItem
    {
        public string value { get; set; }
        public bool isSalary { get; set; }
        public string comment { get; set; }
        public Income originalIncome { get; set; }

        public IncomeDataItem(Income inc)
        {
            value = inc.value.ToString("F");
            isSalary = inc.type == Income.IncomeType.Salary;
            comment = inc.comment;
            originalIncome = inc;
        }
    }
}
