using BudgetManager.Utilities;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Models
{
    internal static class AppData
    {
        public static SortedSet<Month> Months = new SortedSet<Month>();
        public static SortedSet<Quarter> Quarters { get => GetQuarters(); }
        public static HashSet<Category> Categories = new HashSet<Category>();

        public static NavigationView NavigationView;
        public static Frame MainFrame;

        private static int selectedMonth = -1;

        public static Month SelectedMonth { get => Months.ElementAtOrDefault(selectedMonth); set => selectedMonth = Months.ToList().IndexOf(value); }

        public static void Initialize()
        {
            var data = DataReader.Read();
            Logger.Log("read " + data.Length + " characters of app data");
        }

        public static void Save()
        {
            DataReader.Save();
        }

        public static bool IsNotEmpty()
        {
            return Months.Count > 0;
        }

        public static SortedSet<Quarter> GetQuarters()
        {
            var quarters = new SortedSet<Quarter>();
            var monthEnumerator = Months.GetEnumerator();

            while (monthEnumerator.MoveNext() && !monthEnumerator.Current.IsCurrent())
            {
                var month = monthEnumerator.Current;
                if (month.StartDate.Month == 1 ||
                    month.StartDate.Month == 4 ||
                    month.StartDate.Month == 7 ||
                    month.StartDate.Month == 10)
                {
                    var list = new SortedSet<Month>();
                    list.Add(month);
                    if (monthEnumerator.MoveNext())
                    {
                        list.Add(monthEnumerator.Current);
                        if (monthEnumerator.MoveNext())
                        {
                            list.Add(monthEnumerator.Current);
                        }
                    }
                    quarters.Add(new Quarter(list));
                }
            }

            return quarters;
        }

        public static bool IsCategoryRemovable(Category cat)
        {
            foreach (var month in Months)
            {
                foreach (var exp in month.Expenses)
                {
                    if (exp.Category == cat)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        internal static bool SelectPreviousMonth()
        {
            if (selectedMonth != 0)
            {
                selectedMonth--;
                return true;
            }
            return false;
        }

        internal static bool SelectNextMonth()
        {
            if (selectedMonth < Months.Count - 1)
            {
                selectedMonth++;
                return true;
            }
            return false;
        }
    }
}
