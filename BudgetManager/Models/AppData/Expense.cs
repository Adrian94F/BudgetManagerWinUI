using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager
{
    public class Expense
    {
        public DateTime date { get; set; }
        public decimal value { get; set; }
        public string comment { get; set; }
        public Category category { get; set; }
        public bool monthlyExpense { get; set; }

        public Expense()
        {
            date = DateTime.Today;
            monthlyExpense = false;
        }

        public string MonthlyExpense()
        {
            return monthlyExpense ? "↺" : "";
        }

        public string Value()
        {
            return value.ToString("F");
        }

        public override string ToString()
        {
            return date.ToString("dd.MM.yyyy") + " " + value.ToString() + " " + MonthlyExpense() + " " + category.name + " " + comment;
        }
    }
}
