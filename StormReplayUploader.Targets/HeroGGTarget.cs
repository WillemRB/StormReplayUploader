using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;

namespace StormReplayUploader.Targets
{
    /// <summary>
    /// Target that uploads StormReplay files to the hero.gg website.
    /// </summary>
    public class HeroGGTarget : IStormReplayTarget
    {
        private DateTime LastCommit { get { return TargetState.Get(Name); } }

        private HttpClient client;

        public string Name
        {
            get { return "HeroGG"; }
        }

        public HeroGGTarget()
        {
            client = new HttpClient();
        }
        
        public void Notify(FileInfo fileInfo)
        {
            var content = new StreamContent(new StreamReader(fileInfo.FullName).BaseStream);

            try
            {
                var response = client.PostAsync("http://upload.hero.gg/ajax/upload-replay?from=StormReplayUploader", content).Result;

                var message = response.Content.ReadAsStringAsync().Result;

                ReplayPublisher.Log.Information("Target: {0}\nFile: {1}\nResponse: {2}\nResponse content: {3}",
                    this.Name,
                    fileInfo.FullName,
                    response.StatusCode,
                    message);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TargetState.Update(Name, fileInfo.CreationTimeUtc);
                }
            }
            finally
            {
                content.Dispose();
            }
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(file => file.CreationTimeUtc > LastCommit)
                .Subscribe(Notify);
        }
    }
}
