namespace SpotbaseSharp.Services
{
    public interface IConfigService
    {
        void SaveAppSettingsValue(string key, string value);

        string GetAppSettingsValue(string key);
    }
}