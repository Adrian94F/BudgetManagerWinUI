using BudgetManager.Models;
using BudgetManager.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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

namespace BudgetManager.Controls
{
    public sealed partial class ExpDetails : UserControl
    {
        public Expense Expense { get; set; }
        private bool isExpenseNew;

        public EventHandler ExpenseChanged { get; set; }

        private readonly ObservableCollection<Category> expenseCategories = new(AppData.categories);

        public ExpDetails()
        {
            this.InitializeComponent();
            if (Expense == null)
            {
                Expense = new Expense();
            }
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            var picker = sender as DatePicker;
            var dateTimeOffset = picker.SelectedDate;
            if (dateTimeOffset != null)
            {
                var date = dateTimeOffset.Value.Date;
                Expense.Date = date;
                OnExpUpdate();
            }
        }

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var box = sender as NumberBox;
            var number = box.Value;
            if (double.IsNaN(number))
            {
                return;
            }
            var value = Convert.ToDecimal(number);
            Expense.Value = value;
            OnExpUpdate();
        }

        private void CheckBox_CheckedUnchecked(object sender, RoutedEventArgs e)
        {
            var box = sender as CheckBox;
            var value = box.IsChecked;
            Expense.MonthlyExpense = (bool)value;
            OnExpUpdate();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var box = sender as TextBox;
            var text = box.Text;
            Expense.Comment = text;
            OnExpUpdate();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ComboBox;
            var category = box.SelectedItem as Category;
            Expense.Category = category;
            OnExpUpdate();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            AppData.CurrentMonth.Expenses.Remove(Expense);
            OnExpUpdate(onRemoval: true);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AppData.CurrentMonth.Expenses.Add(Expense);
            OnExpUpdate();
        }


        private void OnExpUpdate(bool onRemoval = false)
        {
            if (!onRemoval)
            {
                isExpenseNew = !AppData.CurrentMonth.Expenses.Contains(Expense);
                Logger.Log(isExpenseNew ? "new expense" : "existing expense");

                AddButton.Visibility = isExpenseNew ? Visibility.Visible : Visibility.Collapsed;
                AddButton.IsEnabled = isExpenseNew && isReadyToBeAdded();
            }

            if (!isExpenseNew || onRemoval)
            {
                ExpenseChanged?.Invoke(this, new EventArgs());
            }
        }

        private bool isReadyToBeAdded()
        {
            return DatePicker.SelectedDate != null
                   && ValueNumberBox.Value != 0
                   && ValueNumberBox.Value != double.NaN
                   && CategoryComboBox.SelectedItem != null;
        }
    }
}
