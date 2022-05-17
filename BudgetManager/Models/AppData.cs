using BudgetManager.Utilities;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Models
{
    static class AppData
    {
        public static SortedSet<Month> Months = new SortedSet<Month>();
        public static SortedSet<Quarter> Quarters { get => GetQuarters(); }
        public static HashSet<Category> Categories = new HashSet<Category>();

        public static NavigationView NavigationView;
        public static Frame MainFrame;

        private static int currentMonth = -1;

        public static Month CurrentMonth { get => Months.ElementAtOrDefault(currentMonth); set => currentMonth = Months.ToList().IndexOf(value); }

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

            while (monthEnumerator.MoveNext())
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
                        return false;
                }
            }
            return true;
        }

        internal static bool SelectPreviousMonth()
        {
            if (currentMonth != 0)
            {
                currentMonth--;
                return true;
            }
            return false;
        }

        internal static bool SelectNextMonth()
        {
            if (currentMonth < Months.Count - 1)
            {
                currentMonth++;
                return true;
            }
            return false;
        }
    }
}
