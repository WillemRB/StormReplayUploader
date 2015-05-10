using System;

namespace StormReplayUploader
{
    public class UploaderSettings
    {
        public static DateTime Get(string name)
        {
            var value = Settings.Default[name];

            return value == null ? new DateTime() : (DateTime)value;
        }

        public static void Update(string name, DateTime dateTime)
        {
            Settings.Default[name] = dateTime.ToString();
            Settings.Default.Save();
        }
    }
}
