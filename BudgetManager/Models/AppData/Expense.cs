using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    public class Expense : IComparable<Expense>
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public double Value2 { get => Convert.ToDouble(Value); set => this.Value = Convert.ToDecimal(value); }
        public string Comment { get; set; }
        public Category Category { get; set; }
        public bool MonthlyExpense { get; set; }
        private int ID { get; }

        private static int _counter = 0;

        public Expense()
        {
            Date = DateTime.Today;
            MonthlyExpense = false;
            ID = _counter++;
        }

        public string DateString { get => Date.ToString("dd.MM.yyyy"); }

        public string MonthlyExpenseString { get => MonthlyExpense ? "↺" : ""; }

        public string ValueString { get => Value.ToString("F"); }

        public override string ToString()
        {
            return Date.ToString("s") + " " + Category.Name + " " + ValueString + " " + ID.ToString();
        }

        public int CompareTo(Expense other)
        {
            return ToString().CompareTo(other.ToString());
        }
    }
}
