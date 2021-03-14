using System;
using System.Collections.Generic;
using System.IO;

namespace Map_Editor.Misc
{
    /// <summary>
    /// ConfigurationManager class.
    /// </summary>
    public static class ConfigurationManager
    {
        /// <summary>
        /// Configuration File Name.
        /// </summary>
        private const string FILE_NAME = "Map Editor.ini";

        #region Member Declarations

        /// <summary>
        /// Gets or sets the configuration values.
        /// </summary>
        /// <value>The configuration values.</value>
        private static IDictionary<string, IDictionary<string, object>> configurationValues { get; set; }

        #endregion

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public static void LoadConfig()
        {
            configurationValues = new Dictionary<string, IDictionary<string, object>>();

            if (!File.Exists(FILE_NAME))
                File.Create(FILE_NAME).Close();

            StreamReader streamReader = new StreamReader(File.Open(FILE_NAME, FileMode.Open));

            string currentLine;
            string groupName = string.Empty;

            while ((currentLine = streamReader.ReadLine()) != null)
            {
                if (currentLine.Trim().Length == 0)
                    continue;

                if (currentLine[0] == '[')
                {
                    groupName = currentLine.Substring(1, currentLine.Length - 2);
                    configurationValues.Add(groupName, new Dictionary<string, object>());

                    continue;
                }

                int length;

                for (length = 0; length < currentLine.Length; length++)
                {
                    if (currentLine[length] == '=')
                        break;
                }

                int intValue;
                float floatValue;

                if (currentLine.Substring(length + 1, currentLine.Length - (length + 1)) == "True" || currentLine.Substring(length + 1, currentLine.Length - (length + 1)) == "False")
                    configurationValues[groupName].Add(currentLine.Substring(0, length), Convert.ToBoolean(currentLine.Substring(length + 1, currentLine.Length - (length + 1))));
                else if (int.TryParse(currentLine.Substring(length + 1, currentLine.Length - (length + 1)), out intValue))
                    configurationValues[groupName].Add(currentLine.Substring(0, length), intValue);
                else if (float.TryParse(currentLine.Substring(length + 1, currentLine.Length - (length + 1)), out floatValue))
                    configurationValues[groupName].Add(currentLine.Substring(0, length), floatValue);
                else
                    configurationValues[groupName].Add(currentLine.Substring(0, length), currentLine.Substring(length + 1, currentLine.Length - (length + 1)));
            }

            streamReader.Close();
        }

        /// <summary>
        /// Checks the configuration
        /// </summary>
        public static void CheckConfig()
        {
            if (!configurationValues.ContainsKey("Draw"))
                configurationValues.Add("Draw", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("UI"))
                configurationValues.Add("UI", new Dictionary<string, object>());

            if (!configurationValues["Draw"].ContainsKey("Monsters"))
                configurationValues["Draw"].Add("Monsters", true);
            if (!configurationValues["Draw"].ContainsKey("NPCs"))
                configurationValues["Draw"].Add("NPCs", true);
            if (!configurationValues["Draw"].ContainsKey("SpawnPoints"))
                configurationValues["Draw"].Add("SpawnPoints", true);
            if (!configurationValues["Draw"].ContainsKey("WarpGates"))
                configurationValues["Draw"].Add("WarpGates", true);
            if (!configurationValues["Draw"].ContainsKey("Effects"))
                configurationValues["Draw"].Add("Effects", true);
            if (!configurationValues["Draw"].ContainsKey("Sounds"))
                configurationValues["Draw"].Add("Sounds", true);
            if (!configurationValues["Draw"].ContainsKey("Sky"))
                configurationValues["Draw"].Add("Sky", true);
            if (!configurationValues["Draw"].ContainsKey("Animation"))
                configurationValues["Draw"].Add("Animation", true);
            if (!configurationValues["Draw"].ContainsKey("Collision"))
                configurationValues["Draw"].Add("Collision", true);
            if (!configurationValues["Draw"].ContainsKey("Construction"))
                configurationValues["Draw"].Add("Construction", true);
            if (!configurationValues["Draw"].ContainsKey("Decoration"))
                configurationValues["Draw"].Add("Decoration", true);
            if (!configurationValues["Draw"].ContainsKey("EventTriggers"))
                configurationValues["Draw"].Add("EventTriggers", true);
            if (!configurationValues["Draw"].ContainsKey("Heightmaps"))
                configurationValues["Draw"].Add("Heightmaps", true);
            if (!configurationValues["Draw"].ContainsKey("Water"))
                configurationValues["Draw"].Add("Water", true);
            if (!configurationValues["Draw"].ContainsKey("GridNumbers"))
                configurationValues["Draw"].Add("GridNumbers", true);
            if (!configurationValues["Draw"].ContainsKey("GridOutline"))
                configurationValues["Draw"].Add("GridOutline", true);

            if (!configurationValues["UI"].ContainsKey("OutputExpanded"))
                configurationValues["UI"].Add("OutputExpanded", true);
            if (!configurationValues["UI"].ContainsKey("RightSplitterWidth"))
                configurationValues["UI"].Add("RightSplitterWidth", 300);

            if (!configurationValues.ContainsKey("Decoration"))
                configurationValues.Add("Decoration", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("Construction"))
                configurationValues.Add("Construction", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("EventTriggers"))
                configurationValues.Add("EventTriggers", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("NPCs"))
                configurationValues.Add("NPCs", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("Monsters"))
                configurationValues.Add("Monsters", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("SpawnPoints"))
                configurationValues.Add("SpawnPoints", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("WarpGates"))
                configurationValues.Add("WarpGates", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("Sounds"))
                configurationValues.Add("Sounds", new Dictionary<string, object>());
            if (!configurationValues.ContainsKey("Animation"))
                configurationValues.Add("Animation", new Dictionary<string, object>());

            if (!configurationValues["Decoration"].ContainsKey("DrawBoundingBoxes"))
                configurationValues["Decoration"].Add("DrawBoundingBoxes", true);

            if (!configurationValues["Construction"].ContainsKey("DrawBoundingBoxes"))
                configurationValues["Construction"].Add("DrawBoundingBoxes", true);

            if (!configurationValues["EventTriggers"].ContainsKey("DrawBoundingBoxes"))
                configurationValues["EventTriggers"].Add("DrawBoundingBoxes", true);

            if (!configurationValues["NPCs"].ContainsKey("DrawBoundingBoxes"))
                configurationValues["NPCs"].Add("DrawBoundingBoxes", true);

            if (!configurationValues["Monsters"].ContainsKey("DrawRadii"))
                configurationValues["Monsters"].Add("DrawRadii", true);

            if (!configurationValues["SpawnPoints"].ContainsKey("DrawIndicators"))
                configurationValues["SpawnPoints"].Add("DrawIndicators", true);

            if (!configurationValues["WarpGates"].ContainsKey("DrawWarpAreas"))
                configurationValues["WarpGates"].Add("DrawWarpAreas", true);

            if (!configurationValues["Sounds"].ContainsKey("DrawRadii"))
                configurationValues["Sounds"].Add("DrawRadii", true);

            if (!configurationValues["Animation"].ContainsKey("DrawBoundingBoxes"))
                configurationValues["Animation"].Add("DrawBoundingBoxes", true);

            SaveConfig();
        }

        /// <summary>
        /// Saves the configuration.
        /// </summary>
        public static void SaveConfig()
        {
            StreamWriter streamWriter = new StreamWriter(File.Open(FILE_NAME, FileMode.Truncate));

            foreach (KeyValuePair<string, IDictionary<string, object>> configurationGroup in configurationValues)
            {
                streamWriter.WriteLine("[{0}]", configurationGroup.Key);

                foreach (KeyValuePair<string, object> configurationValue in configurationGroup.Value)
                    streamWriter.WriteLine("{0}={1}", configurationValue.Key, configurationValue.Value);
            }

            streamWriter.Close();
        }

        /// <summary>
        /// Gets the specified value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="group">The group.</param>
        /// <param name="setting">The setting.</param>
        /// <returns></returns>
        public static T GetValue<T>(string group, string setting)
        {
            if (configurationValues.ContainsKey(group) && configurationValues[group].ContainsKey(setting))
                return (T)configurationValues[group][setting];

            throw new Exception("Value does not exist");
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="group">The group.</param>
        /// <param name="setting">The setting.</param>
        /// <param name="value">The value.</param>
        public static void SetValue(string group, string setting, object value)
        {
            configurationValues[group][setting] = value;
        }
    }
}