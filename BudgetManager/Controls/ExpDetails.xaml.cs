using BudgetManager.Models;
using BudgetManager.Utilities;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BudgetManager.Controls
{
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return new DateTimeOffset(((DateTime)value).ToUniversalTime());

        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value != null ? ((DateTimeOffset)value).DateTime : DateTime.Now;
        }
    }

    public sealed partial class ExpDetails : UserControl
    {
        public Expense Expense { get; set; }
        private bool isExpenseNew;

        public EventHandler ExpenseChanged { get; set; }
        public EventHandler ExpenseSaved { get; set; }

        private readonly ObservableCollection<Category> expenseCategories = new(AppData.Categories);

        public ExpDetails()
        {
            this.InitializeComponent();
            if (Expense == null)
            {
                Expense = new Expense();
            }
            ValueNumberBox.Focus(FocusState.Pointer);
        }

        public ExpDetails(DateTime date)
        {
            this.InitializeComponent();
            if (Expense == null)
            {
                Expense = new Expense()
                {
                    Date = date
                };
            }
            ValueNumberBox.Focus(FocusState.Pointer);
        }

        public ExpDetails(Category category)
        {
            this.InitializeComponent();
            if (Expense == null)
            {
                Expense = new Expense()
                {
                    Category = category
                };
            }
            ValueNumberBox.Focus(FocusState.Pointer);
        }

        public ExpDetails(DateTime date, Category category)
        {
            this.InitializeComponent();
            if (Expense == null)
            {
                Expense = new Expense()
                {
                    Category = category,
                    Date = date
                };
            }
            ValueNumberBox.Focus(FocusState.Pointer);
        }

        private void DatePicker_DateChanged(object sender, CalendarDatePickerDateChangedEventArgs e)
        {
            if (e.NewDate.HasValue)
            {
                Expense.Date = e.NewDate.Value.DateTime;
                OnExpUpdate();
            }
        }

        private void NumberBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
        {
            var box = sender;
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
            Remove();
        }

        private void Remove()
        {
            AppData.SelectedMonth.Expenses.Remove(Expense);
            OnExpUpdate(onRemoval: true);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Add();
        }

        private void Add()
        {
            AppData.SelectedMonth.Expenses.Add(Expense);
            OnExpUpdate(onNew: true);
        }

        private void OnExpUpdate(bool onRemoval = false, bool onNew = false)
        {
            if (!onRemoval)
            {
                isExpenseNew = !AppData.SelectedMonth.Expenses.Contains(Expense);
                Logger.Log(isExpenseNew ? "new expense" : "existing expense");

                AddButton.Visibility = isExpenseNew ? Visibility.Visible : Visibility.Collapsed;
                AddButton.IsEnabled = isExpenseNew && IsReadyToBeAdded();
            }

            if (!isExpenseNew || onRemoval)
            {
                ExpenseChanged?.Invoke(this, new EventArgs());
            }
            if (onNew)
            {
                ExpenseSaved?.Invoke(this, new EventArgs());
            }
        }

        private bool IsReadyToBeAdded()
        {
            return DatePicker.Date != null
                   && ValueNumberBox.Value != 0
                   && ValueNumberBox.Value != double.NaN
                   && CategoryComboBox.SelectedItem != null;
        }

        private void KeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            switch (args.KeyboardAccelerator.Key)
            {
                case Windows.System.VirtualKey.Enter:
                    if (AddButton.Visibility == Visibility.Visible && IsReadyToBeAdded())
                    {
                        Add();
                    }

                    break;
                case Windows.System.VirtualKey.Delete:
                    Remove();
                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ValueNumberBox.Focus(FocusState.Keyboard);
        }
    }
}
