using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;

namespace StormReplayUploader.Targets
{
    public class StormLogsTarget : IStormReplayTarget
    {
        private DateTime LastCommit { get { return TargetState.Get(Name); } }

        private HttpClient client;

        public string Name
        {
            get { return "StormLogs"; }
        }

        public StormLogsTarget()
        {
            client = new HttpClient();
        }

        public void Notify(FileInfo fileInfo)
        {
            
            using (var stream = new StreamReader(fileInfo.FullName))
            using (var content = new StreamContent(stream.BaseStream))
            {
                client.PostAsync("http://upload.stormlogs.com/upload.php", content)
                    .ContinueWith(task =>
                    {
                        var response = task.Result;

                        var responseContent = response.Content.ReadAsStringAsync()
                            .ContinueWith(contentTask =>
                            {
                                ReplayPublisher.Log.Information("Target: {0}\nFile: {1}\nResponse: {2}\nResponse content: {3}",
                                        this.Name,
                                        fileInfo.FullName,
                                        response.StatusCode,
                                        contentTask.Result);
                            }
                        );

                        if (response.StatusCode == HttpStatusCode.OK)
                        {
                            TargetState.Update(Name, fileInfo.CreationTimeUtc);
                        }
                    }
                    );
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
