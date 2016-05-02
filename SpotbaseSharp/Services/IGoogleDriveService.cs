using System;
using System.Collections.Generic;
using Google.Apis.Drive.v3.Data;

namespace SpotbaseSharp.Services
{
    public interface IGoogleDriveService
    {
        void CreateApplicationFolder();

        File GetFolder();

        List<File> GetFiles(string folderId);

        void DownloadFile(string fileId, string path);

        bool IsInitalized();

        event EventHandler DownloadCompleted;
    }
}