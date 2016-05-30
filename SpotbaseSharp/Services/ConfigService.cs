using System.Configuration;

namespace SpotbaseSharp.Services
{
    public class ConfigService : IConfigService
    {
        public void SaveAppSettingsValue(string key, string value)
        {
            //Create the object
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            //make changes
            config.AppSettings.Settings[key].Value = value;

            //save to apply changes
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public string GetAppSettingsValue(string key)
        {
            return ConfigurationManager.AppSettings.Get(key);
        }
    }
}