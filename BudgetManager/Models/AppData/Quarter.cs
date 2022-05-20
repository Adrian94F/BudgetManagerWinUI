using System.Collections.Generic;
using System.Linq;

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
            if (months.Last() == AppData.CurrentMonth)
            {
                months.Remove(months.Last());
            }
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
