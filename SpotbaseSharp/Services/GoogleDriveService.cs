using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using GalaSoft.MvvmLight.Messaging;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using SpotbaseSharp.Messages;
using File = Google.Apis.Drive.v3.Data.File;

namespace SpotbaseSharp.Services
{
    public class GoogleDriveService : IGoogleDriveService
    {
        private const string SpotbaseFolder = "SpotbaseSharp";
        private static readonly string[] Scopes = {DriveService.Scope.Drive};
        private readonly Messenger _messenger;
        private readonly DriveService _driveService;
        private bool _isInitalized;

        public GoogleDriveService(Messenger messenger)
        {
            _messenger = messenger;

            UserCredential credential = CreateUserCredential();

            if (credential != null)
            {
                _driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "SpotbaseSharp"
                });

                _isInitalized = true;
            }
        }

        public void CreateApplicationFolder()
        {
            var fileMetadata = new File { Name = SpotbaseFolder, MimeType = "application/vnd.google-apps.folder" };

            var request = _driveService.Files.Create(fileMetadata);
            request.Fields = "id";
            request.Execute();
        }

        public File GetFolder()
        {
            FilesResource.ListRequest listRequest = _driveService.Files.List();
            listRequest.Q = string.Format("mimeType = 'application/vnd.google-apps.folder' and name = '{0}'", SpotbaseFolder);
            listRequest.PageSize = 10;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType)";

            var folder = listRequest.Execute().Files.FirstOrDefault();

            return folder;
        }

        public List<File> GetFiles(string folderId)
        {
            FilesResource.ListRequest listRequest = _driveService.Files.List();
            listRequest.Q = string.Format("'{0}' in parents and mimeType = 'image/jpeg'", folderId);
            listRequest.PageSize = 100;
            listRequest.Fields = "nextPageToken, files(id, name, mimeType)";

            var files = listRequest.Execute().Files;

            return (List<File>) files;
        }

        public void DownloadFile(string fileId, string path)
        {
            //TODO: refactor
            var stream = new FileStream(path, FileMode.Create);

            FilesResource.GetRequest request = _driveService.Files.Get(fileId);

            // Add a handler which will be notified on progress changes.
            // It will notify on each chunk download and when the
            // download is completed or failed.
            request.MediaDownloader.ProgressChanged +=
                progress =>
                {
                    switch (progress.Status)
                    {
                        case DownloadStatus.Completed:
                        {
                            Console.WriteLine("Download completed.");

                            stream.Flush();
                            stream.Close();
                            stream.Dispose();

                            OnDownloadCompleted(this, null);
                            break;
                        }
                        case DownloadStatus.Failed:
                        {
                            Console.WriteLine("Download failed.");
                            break;
                        }
                    }
                };

            request.Download(stream);
        }

        public bool IsInitalized()
        {
            return _isInitalized;
        }

        public event EventHandler DownloadCompleted;

        private void OnDownloadCompleted(object sender, EventArgs e)
        {
            EventHandler downloadCompleted = DownloadCompleted;
            if (downloadCompleted != null)
                downloadCompleted(this, e);
        }

        private UserCredential CreateUserCredential()
        {
            UserCredential credential = null;

            //TODO: try / catch
            try
            {
                using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
                {
                    credential =
                        GoogleWebAuthorizationBroker.AuthorizeAsync(
                            GoogleClientSecrets.Load(stream).Secrets
                            , Scopes
                            , Environment.UserName
                            , CancellationToken.None
                            , new FileDataStore("drive.spotbase.sharp")).Result;
                }
            }
            catch (Exception)
            {
                
            }

            return credential;
        }
    }
}