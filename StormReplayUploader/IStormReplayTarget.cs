using System;

namespace StormReplayUploader
{
    public interface IStormReplayTarget
    {
        string Name { get; }

        void Notify(string path);

        void Subscribe(IObservable<string> observable);
    }
}
