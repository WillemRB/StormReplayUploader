using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Reactive.Linq;
using StormReplayUploader.Config;

namespace StormReplayUploader.Targets
{
    public class EventLogTarget : IStormReplayTarget
    {
        private DateTime LastCommit { get { return UploaderState.Get(Name); } }

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

        public void Notify(FileInfo fileInfo)
        {
            if (!available)
            {
                return;
            }

            var message = String.Format("File: {0}", fileInfo.FullName);
            EventLog.WriteEntry(sourceName, message, EventLogEntryType.Information, 1);
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(f => f.CreationTimeUtc > LastCommit)
                .Subscribe(Notify);
        }
    }
}
