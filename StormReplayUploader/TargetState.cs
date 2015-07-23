using System;
using System.IO;
using System.Text;

namespace StormReplayUploader
{
    /// <summary>
    /// Class containing a Get and Update method that can be used to save the state of a Target.
    /// 
    /// </summary>
    public class TargetState
    {
        private static readonly string Extension = ".sru";

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static DateTime Get(string name)
        {
            var fileName = name + Extension;

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
                ReplayPublisher.Log.Warning("The file {fileName} does not exist. Using default value instead.", fileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                ReplayPublisher.Log.Error("StormReplay Uploader is not authorized to change the file {fileName}.\nException: {exception}",
                    fileName,
                    ex.ToString());
                throw;
            }
            catch (IOException ex)
            {
                ReplayPublisher.Log.Error("An IOException occured during the process of updating the state of the Target {target}.\nException: {exception}", name, ex.ToString());
                throw;
            }

            return DateTime.FromFileTimeUtc(0);
        }

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dateTime"></param>
        public static void Update(string name, DateTime dateTime)
        {
            var fileName = name + Extension;

            try
            {
                using (var stream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    // Simply overwrite the data in the file. 
                    // Because it is a timestamp it can only become longer, so there will never be leftover characters.
                    stream.Seek(0, SeekOrigin.Begin);

                    var data = dateTime.ToFileTimeUtc().ToString();
                    var bytes = Encoding.Default.GetBytes(data);

                    stream.Write(bytes, 0, bytes.Length);
                }

                File.SetAttributes(fileName, File.GetAttributes(fileName) | FileAttributes.Hidden);
            }
            catch (UnauthorizedAccessException ex)
            {
                ReplayPublisher.Log.Error("StormReplay Uploader is not authorized to change the file {fileName}.\nException: {exception}", fileName, ex.ToString());
                throw;
            }
            catch (IOException ex)
            {
                ReplayPublisher.Log.Error("An IOException occured during the process of updating the state of the Target {name}.\nException: {exception}", name, ex.ToString());
                throw;
            }
        }
    }
}
