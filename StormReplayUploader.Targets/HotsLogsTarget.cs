using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace StormReplayUploader.Targets
{
    public class HotsLogsTarget : IStormReplayTarget
    {
        private readonly string ACCESS_KEY_ID = "AKIAIESBHEUH4KAAG4UA";
        private readonly string SECRET_ACCESS_KEY = "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";

        private readonly AmazonS3Client client;

        public string Name
        {
            get { return "HotsLogs"; }
        }

        public HotsLogsTarget()
        {
            client = new AmazonS3Client(ACCESS_KEY_ID, SECRET_ACCESS_KEY);
        }

        public void Notify(FileInfo fileInfo)
        {
            var request = new PutObjectRequest()
            {
                BucketName = "heroesreplays",
                Key = new Guid().ToString("D"),
                FilePath = fileInfo.FullName,
            };

            var response = client.PutObject(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                // Success
            }
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable.Subscribe(Notify);
        }
    }
}
