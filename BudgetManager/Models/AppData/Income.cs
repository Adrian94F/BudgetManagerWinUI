using System;

namespace BudgetManager.Models
{
    public class Income
    {
        public enum IncomeType
        {
            Salary,
            Additional
        };
        public DateTime? Date { get; set; }

        public decimal Value { get => value_; set => value_ = value; }

        private decimal value_;

        public string ValueStr { get => value_.ToString("F"); set => Decimal.TryParse(value, out value_); }
        public IncomeType Type { get; set; }
        public string Comment { get; set; }
        public bool IsSalary
        {
            get => Type == IncomeType.Salary;
            set => Type = value ? IncomeType.Salary : IncomeType.Additional;
        }

        public Income()
        {
            Type = IncomeType.Additional;
        }

        public Income(Income income)
        {
            Type = income.Type;
            Value = income.Value;
            Comment = income.Comment;
            Date = income.Date;
        }
    }
}
