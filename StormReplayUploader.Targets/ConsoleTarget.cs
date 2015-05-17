using System;
using System.IO;
using StormReplayUploader.Config;

namespace StormReplayUploader.Targets
{
    public class ConsoleTarget : IStormReplayTarget
    {
        public string Name
        {
            get { return "ConsoleTarget"; }
        }

        public void Notify(FileInfo fileInfo)
        {
            Console.WriteLine(fileInfo.FullName);
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            var dt = UploaderSettings.Get(Name);

            observable.Subscribe(Notify);
        }
    }
}
