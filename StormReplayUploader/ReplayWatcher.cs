using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormReplayUploader
{
    /// <summary>
    /// Class that monitors a directory for new replay files and notifies all registered targets.
    /// </summary>
    public class ReplayWatcher
    {
        private readonly FileSystemWatcher watcher;

        public ReplayWatcher()
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
