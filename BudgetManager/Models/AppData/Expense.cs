using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    public class Expense : IComparable<Expense>
    {
        public DateTime date { get; set; }
        public decimal value { get; set; }
        public double value2 { get => Convert.ToDouble(value); set => this.value = Convert.ToDecimal(value); }
        public string comment { get; set; }
        public Category category { get; set; }
        public bool monthlyExpense { get; set; }
        private int ID { get; }

        private static int _counter = 0;

        public Expense()
        {
            date = DateTime.Today;
            monthlyExpense = false;
            ID = _counter++;
        }

        public string Date { get => date.ToString("dd.MM.yyyy"); }

        public string MonthlyExpense { get => monthlyExpense ? "↺" : ""; }

        public string Value { get => value.ToString("F"); }

        public override string ToString()
        {
            return date.ToString("s") + " " + category.name + " " + Value + " " + ID.ToString();
        }

        public int CompareTo(Expense other)
        {
            return ToString().CompareTo(other.ToString());
        }
    }
}
