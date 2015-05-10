using System;

namespace StormReplayUploader.Targets
{
    public class ConsoleTarget : IStormReplayTarget
    {
        public void Notify(string path)
        {
            Console.WriteLine(path);
        }

        public void Subscribe(IObservable<string> observable)
        {
            observable.Subscribe(Notify);
        }
    }
}
