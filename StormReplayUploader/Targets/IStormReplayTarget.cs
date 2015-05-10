using System;

namespace StormReplayUploader.Targets
{
    public interface IStormReplayTarget
    {
        void Notify(string path);

        void Subscribe(IObservable<string> observable);
    }
}
