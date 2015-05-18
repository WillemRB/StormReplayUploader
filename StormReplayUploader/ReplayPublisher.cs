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
    public class ReplayPublisher
    {
        private FileSystemWatcher watcher;

        private List<IStormReplayTarget> targets;

        private UploaderConfiguration configuration;

        public ReplayPublisher()
        {
            Logger.Init();

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
            Logger.LogInfo("Service started...");

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

            Logger.LogInfo("Service stopped...");
        }

        /// <summary>
        /// Returns an Observable that creates a new event whenever a new file is created that matches
        /// the filter.
        /// </summary>
        /// <returns></returns>
        private IObservable<FileInfo> FileSystemWatcherObservable()
        {
            watcher = new FileSystemWatcher(configuration.ReplayDirectory, configuration.ReplayFilter);
            watcher.IncludeSubdirectories = true;

            return Observable
                .FromEventPattern<FileSystemEventArgs>(watcher, "Created")
                .Select(evt => new FileInfo(evt.EventArgs.FullPath));
        }

        /// <summary>
        /// Returns an Observable of all the file in a directory matching a filter sorted on
        /// the creation time in ascending order.
        /// </summary>
        /// <remarks>
        /// If the directory where replay files are expected doesn't exist it will be created.
        /// </remarks>
        /// <returns></returns>
        private IObservable<FileInfo> PollingObservable()
        {
            if (!Directory.Exists(configuration.ReplayDirectory))
            {
                Directory.CreateDirectory(configuration.ReplayDirectory);
            }

            return new DirectoryInfo(configuration.ReplayDirectory)
                .EnumerateFiles(configuration.ReplayFilter, SearchOption.AllDirectories)
                .OrderBy(f => f.CreationTimeUtc)
                .ToObservable();
        }
    }
}
