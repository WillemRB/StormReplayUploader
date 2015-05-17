using System;
using System.IO;

namespace StormReplayUploader
{
    public interface IStormReplayTarget
    {
        /// <summary>
        /// The name that is given to the Target.
        /// </summary>
        /// <remarks>
        /// This should be a unique value between other targets.
        /// </remarks>
        string Name { get; }

        /// <summary>
        /// Handle the notification of a new event.
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
