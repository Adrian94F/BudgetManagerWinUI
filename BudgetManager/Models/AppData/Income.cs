using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Models
{
    public class Income
    {
        public enum IncomeType
        {
            Salary,
            Additional
        };

        public decimal Value { get; set; }
        public IncomeType Type { get; set; }
        public string Comment { get; set; }

        public Income()
        {
            Type = IncomeType.Additional;
        }
    }
}
