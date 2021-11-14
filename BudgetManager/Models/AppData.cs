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
        public static void Initialize()
        {
            var data = DataReader.Read();
            Logger.Log("read " + data.Length + " characters of app data");
        }
    }
}
