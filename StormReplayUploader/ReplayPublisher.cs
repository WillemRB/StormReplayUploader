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
    public class ReplayPublisher : IDisposable
    {
        private FileSystemWatcher watcher;

        private List<IStormReplayTarget> targets;

        private UploaderConfiguration configuration;

        private IObservable<FileInfo> observable;

        public ReplayPublisher()
        {
            Logger.Init();

            configuration = LoadConfiguration();
        }

        /// <summary>
        /// When the service starts all the targets are created and will
        /// be subscribed to the Observable.
        /// </summary>
        public void Start()
        {
            //observable = FileSystemWatcherObservable();
            observable = IntervalObservable();

            targets = new List<IStormReplayTarget>();

            foreach (var element in configuration.Targets)
            {
                var target = (TargetElement)element;
                var obj = Activator.CreateInstance(target.AssemblyName, target.TypeName);
                targets.Add(obj.Unwrap() as IStormReplayTarget);
            }

            targets.ForEach(t => t.Subscribe(observable));

            if (watcher != null)
            {
                watcher.EnableRaisingEvents = true;
            }

            Logger.LogInfo("Service started...");
        }

        /// <summary>
        /// Stopping the service 
        /// </summary>
        public void Stop()
        {
            if (watcher != null)
            {
                watcher.EnableRaisingEvents = false;
            }

            foreach(var target in targets)
            {
                if (target is IDisposable)
                {
                    ((IDisposable)target).Dispose();
                }
            }

            targets.Clear();
            observable = null;

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
                Logger.LogError("An error occured during the loading of the configuration.\nException details: {0}",
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

        #region IDisposable Members
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (watcher != null)
                {
                    watcher.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
