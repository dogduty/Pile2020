using Pile.db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Pile
{
    public class Settings
    {
        private static Dictionary<string, string> config;
        private static bool loading = false;

        public static void ClearConfig()
        {
            config = null;
        }

        /// <summary>
        /// Returns default value if key not found.
        /// </summary>
        /// <param name="key">The Key to find in settings dictionary</param>
        /// <param name="defaultValue">The value to return if key not found.</param>
        /// <returns></returns>
        public static string GetValue(string key, string defaultValue)
        {
            while (loading)
                System.Threading.Thread.Sleep(100);

            if (config == null)
                loadValues();

            if (!config.ContainsKey(key))
                return defaultValue;

            return config[key];
        }

        /// <summary>
        /// Throws exception if key not found.
        /// </summary>
        /// <param name="key">The Key to find in settings dictionary</param>
        /// <returns></returns>
        public static string GetValue(string key)
        {
            string value = GetValue(key, null);

            if (!config.ContainsKey(key))
                throw new Exception($"Configuration key {key} not found!");

            return config[key];
        }

        #region Overloads

        public static int GetValue(string key, int defaultValue)
        {
            int.TryParse(GetValue(key, ""), out defaultValue);
            return defaultValue;
        }


        #endregion

        private static void loadValues()
        {
            loading = true;
            using (var db = new pileEntities())
                config = db.Settings.ToDictionary(x => x.Id, x => x.ConfigValue);

            loading = false;
        }
    }
}