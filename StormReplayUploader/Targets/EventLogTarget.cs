using System;
using System.Diagnostics;
using System.Security;

namespace StormReplayUploader.Targets
{
    public class EventLogTarget : IStormReplayTarget
    {
        private readonly string sourceName = "StormReplay Uploader";

        private bool available;

        public string Name
        {
            get { return "EventLogTarget"; }
        }

        public EventLogTarget()
        {
            try
            {
                if (!EventLog.SourceExists(sourceName))
                {
                    EventLog.CreateEventSource(sourceName, "Application");
                }

                available = true;
            }
            catch (SecurityException) 
            {
                available = false;
            }
        }

        public void Notify(string path)
        {
            if (!available)
            {
                return;
            }

            var message = String.Format("File: {0}", path);
            EventLog.WriteEntry(sourceName, message, EventLogEntryType.Information, 1);
        }

        public void Subscribe(IObservable<string> observable)
        {
            observable.Subscribe(Notify);
        }
    }
}
