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

        public static void Save()
        {
            Logger.Log("saving app data");

            var output = new Dictionary<string, object>();

            // categories
            var categories = new List<object>();
            foreach (var cat in AppData.categories)
            {
                // category
                categories.Add(cat.Name);
            }
            output.Add(categoriesKey, categories);

            // billing periods
            var months = new List<object>();
            foreach (var m in AppData.months)
            {
                var month = new Dictionary<string, object>();
                // fields
                month.Add(monthStartDateKey, m.StartDate);
                month.Add(monthEndDateKey, m.EndDate);
                month.Add(monthPlannedSavingsKey, m.PlannedSavings);

                // incomes
                var incomes = new List<Dictionary<string, object>>();

                foreach (var inc in m.Incomes)
                {
                    var income = new Dictionary<string, object>();
                    income.Add(incomeValueKey, inc.Value);
                    var type = inc.Type == Income.IncomeType.Salary
                        ? monthIncomeTypeSalary
                        : "";
                    income.Add(incomeTypeKey, type);
                    if (inc.Comment != null)
                        income.Add(incomeCommentKey, inc.Comment);
                    incomes.Add(income);
                }


                month.Add(monthIncomesKey, incomes);

                // expenses
                var expenses = new List<Dictionary<string, object>>();
                foreach (var exp in m.Expenses)
                {
                    var expense = new Dictionary<string, object>();
                    // fields
                    expense.Add(expenseValueKey, exp.Value);
                    expense.Add(expenseDateKey, exp.Date);
                    expense.Add(expenseCommentKey, exp.Comment ?? "");
                    expense.Add(expenseMonthlyExpenseKey, exp.MonthlyExpense.ToString());
                    expense.Add(expenseCategoryKey, exp.Category.Name);

                    expenses.Add(expense);
                }
                month.Add(monthExpensesKey, expenses);

                months.Add(month);
            }
            output.Add(monthsKey, months);

            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString
            };
            var jsonString = JsonSerializer.Serialize(output, options);
            File.WriteAllTextAsync(AppSettings.dataPath, jsonString);
        }

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
                        Name = cat.ToString()
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
                        var comment = expenseFromJson[expenseCommentKey]?.ToString();
                        var monthlyExpense = expenseFromJson[expenseMonthlyExpenseKey].ToString().Equals("True")
                            ? true
                            : false;
                        var categoryName = expenseFromJson[expenseCategoryKey].ToString();
                        var category = new Category()
                        {
                            Name = categoryName
                        };
                        foreach (var cat in AppData.categories)
                            if (cat.Equals(category))
                            {
                                category = cat;
                                break;
                            }

                        var expense = new Expense()
                        {
                            Value = value,
                            Date = date,
                            Comment = comment,
                            MonthlyExpense = monthlyExpense,
                            Category = category
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
