using System;
using System.IO;

namespace StormReplayUploader
{
    public interface IStormReplayTarget
    {
        string Name { get; }

        void Notify(FileInfo fileInfo);

        void Subscribe(IObservable<FileInfo> observable);
    }
}
