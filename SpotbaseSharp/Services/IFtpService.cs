namespace SpotbaseSharp.Services
{
    public interface IFtpService
    {
        void UploadSpots(string key, string jsonSpots);
    }
}