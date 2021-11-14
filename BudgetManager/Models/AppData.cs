using BudgetManager.Utilities;
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

        public static int currentMonth = -1;

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
