using System;
using System.IO;

namespace StormReplayUploader
{
    public interface IStormReplayTarget
    {
        /// <summary>
        /// The unique name of the Target.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Handle a new StormReplay file.
        /// </summary>
        /// <param name="fileInfo"></param>
        void Notify(FileInfo fileInfo);

        /// <summary>
        /// Subscribe the current target to the Observable. This is also the
        /// place to perform other operations on the Observable specific to the Target.
        /// </summary>
        /// <param name="observable"></param>
        void Subscribe(IObservable<FileInfo> observable);
    }
}
