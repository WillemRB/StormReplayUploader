using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using StormReplayUploader.Targets;

namespace StormReplayUploader
{
    /// <summary>
    /// Class that monitors a directory for new replay files and notifies all registered targets.
    /// </summary>
    public class ReplayWatcher
    {
        private readonly FileSystemWatcher watcher;

        private List<IStormReplayTarget> targets;

        public ReplayWatcher()
        {
            targets = new List<IStormReplayTarget>();

            watcher = new FileSystemWatcher(@"D:\");

            var observable = Observable
                .FromEventPattern<FileSystemEventArgs>(watcher, "Created")
                .Select(evt => evt.EventArgs.FullPath);

            var target = new ConsoleTarget();
            target.Subscribe(observable);

            targets.Add(target);
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
        }
    }
}
