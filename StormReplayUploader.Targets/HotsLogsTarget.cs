﻿using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace StormReplayUploader.Targets
{
    /// <summary>
    /// Target that uploads StormReplay files to the hotslogs.com website.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     HotsLogs runs on AWS. Files are uploaded to a S3 bucket.
    ///     After the file is uploaded a request must be done to the website before
    ///     the file is processed.
    /// </para>
    /// <para>
    ///     The Key property of the PutObjectRequest is used as the name of the
    ///     file on the S3 side.
    ///     The same value for the Key must never be used!
    /// </para>
    /// </remarks>
    public class HotsLogsTarget : IStormReplayTarget, IDisposable
    {
        private DateTime LastCommit { get { return TargetState.Get(Name); } }

        private readonly string ACCESS_KEY_ID = "AKIAIESBHEUH4KAAG4UA";
        private readonly string SECRET_ACCESS_KEY = "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";

        private readonly AmazonS3Client client;

        /// <summary>
        /// If HotsLogs is in Maintenance no new requests will be send.
        /// </summary>
        private bool maintenance;

        public string Name
        {
            get { return "HotsLogs"; }
        }

        public HotsLogsTarget()
        {
            var s3Config = new AmazonS3Config()
            {
                ServiceURL = "s3.amazonaws.com",
                RegionEndpoint = Amazon.RegionEndpoint.USEast1,
            };

            client = new AmazonS3Client(ACCESS_KEY_ID, SECRET_ACCESS_KEY, s3Config);
        }

        /// <summary>
        /// Uploads the replay file to a S3 bucket and notifies the HotsLogs website.
        /// </summary>
        /// <param name="fileInfo"></param>
        public void Notify(FileInfo fileInfo)
        {
            var request = new PutObjectRequest()
            {
                BucketName = "heroesreplays",
                Key = Guid.NewGuid().ToString("D") + ".StormReplay",
                FilePath = fileInfo.FullName,
            };

            var response = client.PutObject(request);

            if (response.HttpStatusCode == HttpStatusCode.OK)
            {
                NotifyHotsLogs(request.Key)
                    .ContinueWith(success =>
                    {
                        if (success.Result)
                        {
                            TargetState.Update(Name, fileInfo.CreationTimeUtc);
                        }
                    });
            }
            else
            {
                ReplayPublisher.Log.Warning("File {filePath} was no succesfully uploaded to the HotsLogs S3 bucket.\nReturned statuscode: {statusCode}", fileInfo.FullName, response.HttpStatusCode);
            }
        }

        /// <summary>
        /// Send a request to the HotsLogs website to notify of a new replay file being uploaded.
        /// </summary>
        /// <param name="fileName">The name of the uploaded file</param>
        /// <returns>If the request is successful</returns>
        private async Task<bool> NotifyHotsLogs(string fileName)
        {
            var uri = new Uri("https://www.hotslogs.com/UploadFile.aspx?FileName=" + fileName);

            using (var httpClient = new HttpClient())
            {
                var httpRequest = new HttpRequestMessage()
                {
                    Method = HttpMethod.Get,
                    RequestUri = uri,
                };

                var httpResponse = await httpClient.SendAsync(httpRequest);

                var response = await httpResponse.Content.ReadAsStringAsync();

                ReplayPublisher.Log.Information("Send a request to {uri}.\nResponse: {response}", uri, response);

                // If HotsLogs is in maintenance, stop processing files.
                if (response.Equals("Maintenance"))
                {
                        maintenance = true;
                        return false;
                }

                // Even though there are a number of other states, the only thing
                // that matters is whether a file was succesfully uploaded.
                return true;
            }
        }

        public void Subscribe(IObservable<FileInfo> observable)
        {
            observable
                .Where(f => f.CreationTimeUtc > LastCommit && !maintenance)
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
