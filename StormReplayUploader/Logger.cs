using System;
using System.Diagnostics;

namespace StormReplayUploader
{
    public class Logger
    {
        private static readonly string Source = "StormReplay Uploader";

        private static readonly string LogName = "Application";

        /// <summary>
        /// Create the event source in the EventLog if it doesn't exist.
        /// </summary>
        /// <remarks>
        /// If the source doesn't exist the user needs permission to create 
        /// a new eventlog source. Running as Administrator will suffice.
        /// </remarks>
        public static void Init()
        {
            if (!EventLog.Exists(Source))
            {
                EventLog.CreateEventSource(Source, LogName, ".");
            }
        }

        /// <summary>
        /// Log an informational message to the EventLog.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="args"></param>
        public static void LogInfo(string info, params object[] args)
        {
            var message = String.Format(info, args);
            EventLog.WriteEntry(Source, message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Log a warning message to the EventLog.
        /// </summary>
        /// <param name="warning"></param>
        /// <param name="args"></param>
        public static void LogWarning(string warning, params object[] args)
        {
            var message = String.Format(warning, args);
            EventLog.WriteEntry(Source, message, EventLogEntryType.Warning);
        }

        /// <summary>
        /// Log an error message to the EventLog.
        /// </summary>
        /// <param name="error"></param>
        /// <param name="args"></param>
        public static void LogError(string error, params object[] args)
        {
            var message = String.Format(error, args);
            EventLog.WriteEntry(Source, message, EventLogEntryType.Error);
        }
    }
}
