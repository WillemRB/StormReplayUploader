using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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

            configuration = LoadConfiguration();
            targets = new List<IStormReplayTarget>();

            foreach (var element in configuration.Targets)
            {
                var target = (TargetElement)element;
                var obj = Activator.CreateInstance(target.AssemblyName, target.TypeName);
                targets.Add(obj.Unwrap() as IStormReplayTarget);
            }

            //var observable = FileSystemWatcherObservable();
            var observable = IntervalObservable();

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
        /// Returns the configuration section from the app.config.
        /// Configuration exceptions will be rethrown.
        /// </summary>
        /// <returns></returns>
        private UploaderConfiguration LoadConfiguration()
        {
            try
            {
                return (UploaderConfiguration)ConfigurationManager.GetSection("uploaderConfiguration");
            }
            catch (ConfigurationErrorsException ex)
            {
                Logger.LogError("An error occured during the loading of the configuration.{0}Exception details: {1}",
                    Environment.NewLine,
                    ex.ToString());

                throw;
            }
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
        /// Returns an Observable every update interval. This returned Observable
        /// contains all the StormReplay files in the configured folder ordered by
        /// creation time in ascending order.
        /// </summary>
        /// <returns></returns>
        private IObservable<FileInfo> IntervalObservable()
        {
            if (!Directory.Exists(configuration.ReplayDirectory))
            {
                Directory.CreateDirectory(configuration.ReplayDirectory);
            }

            return Observable
                .Timer(TimeSpan.FromSeconds(configuration.UpdateInterval))
                .SelectMany(
                    Observable
                        .Defer(() =>
                            new DirectoryInfo(configuration.ReplayDirectory)
                                .EnumerateFiles(configuration.ReplayFilter, SearchOption.AllDirectories)
                                .OrderBy(f => f.CreationTimeUtc)
                                .ToObservable()
                        )
                )
                .Repeat();
        }
    }
}
