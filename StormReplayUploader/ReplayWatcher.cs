using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
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

        private UploaderConfiguration configuration;

        public ReplayWatcher()
        {
            configuration = new UploaderConfiguration();
            targets = new List<IStormReplayTarget>();

            watcher = new FileSystemWatcher(configuration.DefaultReplayDirectory, configuration.ReplayFilter);

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
