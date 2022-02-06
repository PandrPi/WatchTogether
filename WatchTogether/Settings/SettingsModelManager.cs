using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;

namespace WatchTogether.Settings
{
    internal class SettingsModelManager
    {
        private static readonly string SettingsFilePath = "settings.json";
        private SettingsModel settings;

        public static SettingsModelManager Instance { get; private set; }

        public static SettingsModel CurrentSettings
        {
            get => Instance.settings;
            set
            {
                Instance.settings = value;
                Instance.SaveSettings();
            }
        }

        public SettingsModelManager()
        {
            Instance = this;

            LoadSettings();
        }

        /// <summary>
        /// Loads the settings from a file. If the loading process fails it creates a new SettingsWT object
        /// </summary>
        public void LoadSettings()
        {
            try
            {
                var settingsData = File.ReadAllText(SettingsFilePath);
                settings = LoadSettingsFromJson(settingsData);
            }
            catch
            {
                settings = new SettingsModel();
            }
        }

        /// <summary>
        /// Saves the current SettingsWT object to file. If the saving process
        /// fails it shows the error message
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                var settingsData = SerializeSettings(settings);
                File.WriteAllText(SettingsFilePath, settingsData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}, {ex.StackTrace}");
                MessageBox.Show(ex.StackTrace, ex.Message);
            }
        }

        private SettingsModel LoadSettingsFromJson(string jsonData)
        {
            return JsonConvert.DeserializeObject<SettingsModel>(jsonData);
        }

        private string SerializeSettings(SettingsModel model)
        {
            return JsonConvert.SerializeObject(model, Formatting.Indented);
        }
    }
}
