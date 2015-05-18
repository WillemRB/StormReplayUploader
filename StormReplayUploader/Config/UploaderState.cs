using System;
using System.IO;
using System.Text;

namespace StormReplayUploader.Config
{
    /// <summary>
    /// Class containing a Get and Update method that can be used to save the state of a Target.
    /// 
    /// </summary>
    public class UploaderState
    {
        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime Get(string name)
        {
            var fileName = name + ".sru";

            try
            {
                var data = File.ReadAllText(fileName);

                long filetime;
                if (long.TryParse(data, out filetime))
                {
                    return DateTime.FromFileTimeUtc(filetime);
                }
            }
            catch (FileNotFoundException)
            {
                // Probably first run or the file was removed. 
                Logger.LogWarning("The file {0} does not exist. Using default value instead.",
                    fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError("StormReplay Uploader is not authorized the change the file {0}.{1}Exception: {2}",
                    fileName,
                    Environment.NewLine,
                    ex.ToString());
            }
            catch (IOException ex)
            {
                Logger.LogError("An IOException occured during the process of updating the state of the Target {0}.{1}Exception: {2}",
                    name,
                    Environment.NewLine,
                    ex.ToString());
            }

            return DateTime.FromFileTimeUtc(0);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dateTime"></param>
        public static void Update(string name, DateTime dateTime)
        {
            var fileName = name + ".sru";

            try
            {
                using (var stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    stream.Seek(0, SeekOrigin.Begin);

                    var data = dateTime.ToFileTimeUtc().ToString();
                    var bytes = Encoding.Default.GetBytes(data);

                    stream.Write(bytes, 0, bytes.Length);
                }

                File.SetAttributes(fileName, FileAttributes.Hidden);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.LogError("StormReplay Uploader is not authorized the change the file {0}.{1}Exception: {2}",
                    fileName,
                    Environment.NewLine,
                    ex.ToString());
            }
            catch (IOException ex)
            {
                Logger.LogError("An IOException occured during the process of updating the state of the Target {0}.{1}Exception: {2}",
                    name,
                    Environment.NewLine,
                    ex.ToString());
            }
        }
    }
}
