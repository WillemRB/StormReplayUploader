using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using StormReplayUploader.Config;

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
            configuration = (UploaderConfiguration)ConfigurationManager.GetSection("uploaderConfiguration");
            targets = new List<IStormReplayTarget>();

            foreach (var element in configuration.Targets)
            {
                var target = (TargetElement)element;
                var obj = Activator.CreateInstance(target.AssemblyName, target.TypeName);
                targets.Add(obj.Unwrap() as IStormReplayTarget);
            }

            //var observable = FileSystemWatcherObservable();
            var observable = PollingObservable();

            targets.ForEach(t => t.Subscribe(observable));
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
