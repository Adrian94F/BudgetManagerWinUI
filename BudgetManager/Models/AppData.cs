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
        public static SortedSet<Month> months = new SortedSet<Month>();
        public static HashSet<Category> categories = new HashSet<Category>();

        public static NavigationView navigationView;
        public static Frame mainFrame;

        public static int currentMonth = -1;

        public static Month CurrentMonth {get => months.ElementAtOrDefault(currentMonth); set => currentMonth = months.ToList().IndexOf(value);}

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
            return months.Count > 0;
        }

        public static bool IsCategoryRemovable(Category cat)
        {
            foreach (var month in months)
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
            if (currentMonth < months.Count - 1)
                currentMonth++;
        }
    }
}
