using System;
using System.IO;
using System.Net;

namespace SpotbaseSharp.Services
{
    public class FtpService : IFtpService
    {
        private const string LastUpdateJson = "{{\"lastUpdate\": \"{0}\" }}";
        private readonly IConfigService _configService;
        private readonly NetworkCredential _credential;

        public FtpService(IConfigService configService)
        {
            _configService = configService;
            _credential = new NetworkCredential("username", "password");
        }

        public void UploadSpots(string key, string jsonSpots)
        {
            const string baseUrl = "domain";

            using (var client = new WebClient())
            {
                string tmpFile = Path.Combine(Path.GetTempPath(), SpotbaseConstants.SpotsJsonFilename);
                File.WriteAllText(tmpFile, jsonSpots);

                client.Credentials = _credential;

                bool isDirectoryCreated =
                    Convert.ToBoolean(_configService.GetAppSettingsValue(AppSettingConstants.IsMobileDirectoryCreated));

                if (!isDirectoryCreated)
                {
                    FtpCreateDirectory(string.Format(baseUrl, key));
                }

                string directoryUrl = string.Format(baseUrl, key);
                client.UploadFile(
                    string.Format("{0}/{1}", directoryUrl, SpotbaseConstants.SpotsJsonFilename),
                    "STOR", tmpFile);

                string tmpFile2 = Path.Combine(Path.GetTempPath(), SpotbaseConstants.LastUpdateJsonFilename);
                File.WriteAllText(tmpFile2, string.Format(LastUpdateJson, DateTime.Now.ToString("dd.MM.yyyyThh:mm:ss")));

                client.UploadFile(
                    string.Format("{0}/{1}", directoryUrl, SpotbaseConstants.LastUpdateJsonFilename),
                    "STOR", tmpFile2);

                File.Delete(tmpFile);
                File.Delete(tmpFile2);
            }
        }

        private void FtpCreateDirectory(string ftpDirectory)
        {
            try
            {
                var request = (FtpWebRequest) WebRequest.Create(ftpDirectory);
                request.Credentials = _credential;
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                using (request.GetResponse())
                {
                    // Okay.
                    _configService.SaveAppSettingsValue(AppSettingConstants.IsMobileDirectoryCreated, "true");
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var response = (FtpWebResponse) ex.Response;
                    if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                    {
                        // Directory not found.  
                    }
                }
            }
        }
    }
}