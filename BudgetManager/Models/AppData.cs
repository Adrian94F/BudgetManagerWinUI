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
        public static HashSet<Category> Categories = new HashSet<Category>();

        public static NavigationView NavigationView;
        public static Frame MainFrame;

        private static int currentMonth = -1;

        public static Month CurrentMonth {get => Months.ElementAtOrDefault(currentMonth); set => currentMonth = Months.ToList().IndexOf(value);}

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

        internal static void SelectPreviousMonth()
        {
            if (currentMonth != 0)
                currentMonth--;
        }

        internal static void SelectNextMonth()
        {
            if (currentMonth < Months.Count - 1)
                currentMonth++;
        }
    }
}
