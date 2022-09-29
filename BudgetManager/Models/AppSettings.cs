using BudgetManager.Utilities;
using Windows.Storage;

namespace BudgetManager.Models
{
    internal static class AppSettings
    {
        private static string dataPathKey = "data path";

        public static string dataPath = "C:\\Users\\adria\\OneDrive\\Dokumenty\\Budget Manager\\dataset.json";

        public static void Save()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values[dataPathKey] = dataPath;
            Logger.Log("save settings - data path: " + dataPath);
        }

        public static void Read()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            var value = localSettings.Values[dataPathKey] as string;
            if (value != null && value != "")
            {
                dataPath = value;
            }
            else
            {
                Logger.Log("empty data path, default kept");
            }
            Logger.Log("read settings - data path: " + dataPath);
        }
    }
}
