using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Models
{
    public class Quarter : Month
    {
        public Quarter(SortedSet<Month> months)
        {
            if (months.Count == 0 || months.Count > 3)
            {
                return;
            }
            StartDate = months.First().StartDate;
            EndDate = months.Last().EndDate;
            foreach (Month month in months)
            {
                foreach (var exp in month.Expenses)
                {
                    Expenses.Add(exp);
                }
                foreach (var inc in month.Incomes)
                {
                    Incomes.Add(inc);
                }
            }
        }
    }
}
