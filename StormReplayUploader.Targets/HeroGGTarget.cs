using System;
using System.IO;
using System.Net;
using System.Net.Http;

namespace StormReplayUploader.Targets
{
    /// <summary>
    /// Target that uploads StormReplay files to the hero.gg website.
    /// </summary>
    public class HeroGGTarget : IStormReplayTarget
    {
        public string Name
        {
            get { return "HeroGG"; }
        }

        public async void Notify(FileInfo fileInfo)
        {
            using (var client = new HttpClient())
            using (var stream = new StreamReader(fileInfo.FullName))
            {
                var content = new StreamContent(stream.BaseStream);

                var response = await client.PostAsync("http://upload.hero.gg/ajax/upload-replay", content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Success
                }
            }
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable.Subscribe(Notify);
        }
    }
}
