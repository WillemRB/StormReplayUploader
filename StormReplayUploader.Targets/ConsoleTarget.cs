using System;
using System.IO;
using System.Reactive.Linq;

namespace StormReplayUploader.Targets
{
    /// <summary>
    /// Target that logs the filename of a StormReplay file to the Console.
    /// </summary>
    /// <remarks>
    /// Not useful outside of testing and debugging.
    /// </remarks>
    public class ConsoleTarget : IStormReplayTarget
    {
        private DateTime LastCommit { get { return TargetState.Get(Name); } }

        public string Name
        {
            get { return "ConsoleTarget"; }
        }

        public void Notify(FileInfo fileInfo)
        {
            Console.WriteLine(fileInfo.FullName);

            TargetState.Update(Name, fileInfo.CreationTimeUtc);
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(file => file.CreationTimeUtc > LastCommit)
                .Subscribe(Notify);
        }
    }
}
