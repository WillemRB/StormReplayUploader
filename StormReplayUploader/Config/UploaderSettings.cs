using System;
using System.Configuration;
using System.Linq;

namespace StormReplayUploader.Config
{
    public class UploaderSettings
    {
        /// <summary>
        /// Returns the DateTime value of a setting.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime Get(string name)
        {
            try
            {
                var value = (long)Settings.Default[name];

                return DateTime.FromFileTimeUtc(value);
            }
            catch (SettingsPropertyNotFoundException)
            {
                return DateTime.FromFileTimeUtc(0);
            }
        }

        /// <summary>
        /// Updates a value in the settings file.
        /// If no setting exists it will be created.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dateTime"></param>
        public static void Update(string name, DateTime dateTime)
        {
            Settings.Default[name] = dateTime.ToFileTimeUtc();
            Settings.Default.Save();
        }
    }
}
