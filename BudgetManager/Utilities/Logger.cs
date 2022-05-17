using System;
using System.Collections.Generic;

namespace BudgetManager.Utilities
{
    internal static class Logger
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
