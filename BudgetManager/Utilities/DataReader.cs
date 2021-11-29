using BudgetManager.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace BudgetManager.Utilities
{
    static class DataReader
    {
        private static readonly string categoriesKey = "categories";
        private static readonly string monthsKey = "billingPeriods";
        private static readonly string monthStartDateKey = "start";
        private static readonly string monthEndDateKey = "end";
        private static readonly string monthIncomesKey = "incomes";
        private static readonly string incomeValueKey = "value";
        private static readonly string incomeTypeKey = "type";
        private static readonly string monthIncomeTypeSalary = "salary";
        private static readonly string incomeCommentKey = "comment";
        private static readonly string monthNetIncomeKey = "netIncome";
        private static readonly string monthAdditionalIncomeKey = "additionalIncome";
        private static readonly string monthPlannedSavingsKey = "plannedSavings";
        private static readonly string monthExpensesKey = "expenses";
        private static readonly string expenseValueKey = "value";
        private static readonly string expenseDateKey = "date";
        private static readonly string expenseCommentKey = "comment";
        private static readonly string expenseMonthlyExpenseKey = "monthlyExpense";
        private static readonly string expenseCategoryKey = "category";

        public static string Read()
        {
            Logger.Log("reading app data");
            var pathToDataSet = AppSettings.dataPath;
            if (File.Exists(pathToDataSet))
            {
                var sr = new StreamReader(pathToDataSet);
                string jsonString = sr.ReadToEnd();
                sr.Close();

                var dataSet = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

                // categories
                var categories = JsonSerializer.Deserialize<List<object>>(dataSet[categoriesKey].ToString());
                foreach (var cat in categories)
                {
                    var category = new Category()
                    {
                        name = cat.ToString()
                    };
                    AppData.categories.Add(category);
                }
                Logger.Log("read " + AppData.categories.Count + " categories");

                // months
                var months = JsonSerializer.Deserialize<List<object>>(dataSet[monthsKey].ToString());
                foreach (var bp in months)
                {
                    var monthFromJson = JsonSerializer.Deserialize<Dictionary<string, object>>(bp.ToString());

                    var startDate = DateTime.Parse(monthFromJson[monthStartDateKey].ToString());
                    var endDate = DateTime.Parse(monthFromJson[monthEndDateKey].ToString());
                    var plannedSavings = Decimal.Parse(monthFromJson[monthPlannedSavingsKey].ToString(), CultureInfo.InvariantCulture);

                    var month = new Month()
                    {
                        StartDate = startDate,
                        EndDate = endDate,
                        PlannedSavings = plannedSavings
                    };

                    // new incomes
                    if (monthFromJson.Keys.Contains(monthIncomesKey))
                    {
                        var incomes = JsonSerializer.Deserialize<List<object>>(monthFromJson[monthIncomesKey].ToString());
                        foreach (var inc in incomes)
                        {
                            var incomeFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(inc.ToString());
                            var value = Decimal.Parse(incomeFromJson[incomeValueKey].ToString(), CultureInfo.InvariantCulture);
                            if (value == 0)
                                continue;
                            var typeString = incomeFromJson[incomeTypeKey];
                            var type = typeString.Equals(monthIncomeTypeSalary)
                                ? Income.IncomeType.Salary
                                : Income.IncomeType.Additional;
                            var comment = incomeFromJson.Keys.Contains(incomeCommentKey)
                                ? incomeFromJson[incomeCommentKey]
                                : "";

                            var income = new Income()
                            {
                                Value = value,
                                Type = type,
                                Comment = comment
                            };
                            month.Incomes.Add(income);
                        }
                    }
                    else
                    {
                        if (monthFromJson.Keys.Contains(monthNetIncomeKey))
                        {
                            var income = new Income()
                            {
                                Value = Decimal.Parse(monthFromJson[monthNetIncomeKey].ToString(), CultureInfo.InvariantCulture),
                                Type = Income.IncomeType.Salary
                            };
                            if (income.Value > decimal.Zero)
                                month.Incomes.Add(income);
                        }
                        if (monthFromJson.Keys.Contains(monthAdditionalIncomeKey))
                        {
                            var income = new Income()
                            {
                                Value = Decimal.Parse(monthFromJson[monthAdditionalIncomeKey].ToString(), CultureInfo.InvariantCulture),
                                Type = Income.IncomeType.Additional
                            };
                            if (income.Value > decimal.Zero)
                                month.Incomes.Add(income);
                        }
                    }

                    //expenses
                    var expenses = JsonSerializer.Deserialize<List<object>>(monthFromJson[monthExpensesKey].ToString());
                    foreach (var exp in expenses)
                    {
                        var expenseFromJson = JsonSerializer.Deserialize<Dictionary<string, string>>(exp.ToString());

                        var value = Decimal.Parse(expenseFromJson[expenseValueKey].ToString(), CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(expenseFromJson[expenseDateKey].ToString());
                        var comment = expenseFromJson[expenseCommentKey].ToString();
                        var monthlyExpense = expenseFromJson[expenseMonthlyExpenseKey].ToString().Equals("True")
                            ? true
                            : false;
                        var categoryName = expenseFromJson[expenseCategoryKey].ToString();
                        var category = new Category()
                        {
                            name = categoryName
                        };
                        foreach (var cat in AppData.categories)
                            if (cat.Equals(category))
                            {
                                category = cat;
                                break;
                            }

                        var expense = new Expense()
                        {
                            value = value,
                            date = date,
                            comment = comment,
                            monthlyExpense = monthlyExpense,
                            category = category
                        };
                        month.Expenses.Add(expense);
                    }

                    AppData.months.Add(month);
                }
                Logger.Log("read " + AppData.months.Count + " months");
                AppData.currentMonth = AppData.months.Count - 1;

                return jsonString;
            }
            Logger.Log("couldn't read app data");
            return "";
        }
    }
}
