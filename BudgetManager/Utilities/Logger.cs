using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BudgetManager.Utilities
{
    static class Logger
    {
        private static List<string> log = new List<string>();
        public static void Log(string msg)
        {
            var time = DateTime.Now.ToString("R");
            var line = time + "\t" + msg;
            log.Add(line);
        }

        public static string GetLog()
        {
            return string.Join("\n", log);
        }
    }
}
