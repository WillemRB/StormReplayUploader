using System;
using System.IO;
using System.Reactive.Linq;
using StormReplayUploader.Config;

namespace StormReplayUploader.Targets
{
    public class ConsoleTarget : IStormReplayTarget
    {
        private DateTime LastCommit { get { return UploaderSettings.Get(Name); } }

        public string Name
        {
            get { return "ConsoleTarget"; }
        }

        public void Notify(FileInfo fileInfo)
        {
            Console.WriteLine(fileInfo.FullName);

            UploaderSettings.Update(Name, fileInfo.CreationTimeUtc);
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(file => file.CreationTimeUtc > LastCommit)
                .Subscribe(Notify);
        }
    }
}
