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

        public static bool IsNotEmpty()
        {
            return months.Count > 0;
        }
    }
}
