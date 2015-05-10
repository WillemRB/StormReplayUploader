using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using StormReplayUploader.Config;
using StormReplayUploader.Targets;

namespace StormReplayUploader
{
    /// <summary>
    /// Class that monitors a directory for new replay files and notifies all registered targets.
    /// </summary>
    public class ReplayWatcher
    {
        private FileSystemWatcher watcher;

        private List<IStormReplayTarget> targets;

        private UploaderConfiguration configuration;

        public ReplayWatcher()
        {
            configuration = new UploaderConfiguration();
            targets = new List<IStormReplayTarget>();

            //var observable = FileSystemWatcherObservable();
            var observable = PollingObservable();

            var target = new ConsoleTarget();
            target.Subscribe(observable);
            targets.Add(target);
        }

        public void Start()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = true;
            }
        }

        public void Stop()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
            }
        }

        private IObservable<string> FileSystemWatcherObservable()
        {
            watcher = new FileSystemWatcher(configuration.DefaultReplayDirectory, configuration.ReplayFilter);
            watcher.IncludeSubdirectories = true;

            return Observable
                .FromEventPattern<FileSystemEventArgs>(watcher, "Created")
                .Select(evt => evt.EventArgs.FullPath);
        }

        private IObservable<string> PollingObservable()
        {
            return new DirectoryInfo(configuration.DefaultReplayDirectory)
                .EnumerateFiles(configuration.ReplayFilter, SearchOption.AllDirectories)
                .OrderBy(f => f.CreationTimeUtc)
                .Select(f => f.FullName)
                .ToObservable();
        }
    }
}
