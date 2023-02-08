using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Toolbox
{
    /// <summary>
    /// Loads and saves app-wide settings including the location of the database directory to use.
    /// </summary>
    public static class Config
    {
        #region ID strings for commonly accessed config data
        #endregion

        public static readonly string ConfigFilename = "config.ini";
        public static readonly string AppPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string ConfigPath => Path.Combine(AppPath, ConfigFilename);
        static readonly Dictionary<string, string> ConfigMapStr = new Dictionary<string, string>(10);



        /// <summary>
        /// Reads a configuration value. If the value does not exist null is returned.
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static string? ReadConfigStr(string configId, string? defaultValue = null)
        {
            if (ConfigMapStr.TryGetValue(configId, out string? value))
                return value;
            else if(!string.IsNullOrEmpty(defaultValue))
            {
                WriteConfigStr(configId, defaultValue);
                SaveConfig();
                return defaultValue;
            }
            return null;
        }

        /// <summary>
        /// Changes a configuration value.
        /// </summary>
        /// <param name="configId"></param>
        /// <param name="value"></param>
        public static void WriteConfigStr(string configId, string value)
        {
            ConfigMapStr[configId] = value;
        }


        static readonly StringBuilder FileoutputText = new(100);
        /// <summary>
        /// Writes current app settings back to a config file located in the app's root directory.
        /// </summary>
        public static void SaveConfig()
        {
            FileoutputText.Clear();

            foreach (var kvp in ConfigMapStr)
            {
                FileoutputText.Append(kvp.Key);
                FileoutputText.Append('=');
                FileoutputText.Append(kvp.Value);
                FileoutputText.Append(Environment.NewLine);
            }

            File.WriteAllText(ConfigPath, FileoutputText.ToString());
        }

        /// <summary>
        /// Loads a config file from the app's root directory.
        /// </summary>
        public static void LoadConfig()
        {
            string? text = null;
            try
            {
                text = File.ReadAllText(ConfigPath);
            }
            catch (Exception)
            {
                return;
            }
            if (string.IsNullOrEmpty(text)) return;


            var lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).Select(x => x.Trim());

            foreach (var readLine in lines)
            {
                var line = readLine.Trim();
                if (string.IsNullOrEmpty(line)) continue; //skip empty lines
                if (line.StartsWith("//")) continue; //skip comment lines

                var parts = line.Split('=').Select(x => x.Trim()).ToArray();
                if (parts == null || parts.Length != 2) continue;

                string key = parts[0];
                string value = parts[1];
                ConfigMapStr[key] = value;
            }
        }


    }

}
