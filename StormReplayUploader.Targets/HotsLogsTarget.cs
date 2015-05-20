using System;
using System.IO;
using System.Net;
using System.Reactive.Linq;
using Amazon.S3;
using Amazon.S3.Model;

namespace StormReplayUploader.Targets
{
    /// <summary>
    /// Target that uploads StormReplay files to the hotslogs.com website.
    /// </summary>
    /// <remarks>
    /// HotsLogs runs on AWS. Files are uploaded to a S3 bucket.
    /// </remarks>
    public class HotsLogsTarget : IStormReplayTarget, IDisposable
    {
        private DateTime LastCommit { get { return TargetState.Get(Name); } }

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
                TargetState.Update(Name, fileInfo.CreationTimeUtc);
            }
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(f => f.CreationTimeUtc > LastCommit)
                .Subscribe(Notify);
        }

        #region IDisposable Members
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                client.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
