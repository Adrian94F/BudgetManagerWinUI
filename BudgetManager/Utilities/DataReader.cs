using BudgetManager.Models;
using System;
using System.Collections.Generic;
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
        public static string Read()
        {
            var pathToDataSet = AppSettings.dataPath;
            if (File.Exists(pathToDataSet))
            {
                var sr = new StreamReader(pathToDataSet);
                string jsonString = sr.ReadToEnd();
                sr.Close();

                var dataSet = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);

                return jsonString;
            }
            return "";
        }
    }
}
