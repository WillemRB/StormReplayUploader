using System;
using System.Threading;
using StormReplayUploader.Config;

namespace StormReplayUploader.Targets
{
    public class ConsoleTarget : IStormReplayTarget
    {
        public string Name
        {
            get { return "ConsoleTarget"; }
        }

        public void Notify(string path)
        {
            Console.WriteLine(path);
        }

        public void Subscribe(IObservable<string> observable)
        {
            var dt = UploaderSettings.Get(Name);

            observable.Subscribe(Notify);
        }
    }
}
