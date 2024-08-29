using MySaleForceBackup.Interfaces;
using SalesForceBackup;
using System.Configuration;


namespace MySaleForceBackup
{
    /// <summary>
    /// Reads the App.Config application settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler _errorHandler;
        private  readonly Dictionary<string, string> ValuePairs = new Dictionary<string, string>();


        public AppSettings(IErrorHandler errorHandler, IConfiguration configuration)
        {
            _errorHandler = errorHandler;
            _configuration = configuration;
            ReadAllSettings();
        }

        private void ReadAllSettings()
        {
            try
            {
                var appSettings = System.Configuration.ConfigurationManager.AppSettings; ;

                if (appSettings.Count == 0) return;
                foreach (var key in appSettings.AllKeys)
                {
                    ValuePairs.Add(key, appSettings[key]);
                }
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("Error reading app settings");
                _errorHandler.HandleError(e, (int)ExitCode.ConfigurationError);
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
        }


        public string Get(string key)
        {
            string value;
            ValuePairs.TryGetValue(key, out value);
            return value;
        }


        public void Set(string key, string value)
        {
            if (ValuePairs.ContainsKey(key))
            {
                ValuePairs.Remove(key);
            }
            ValuePairs.Add(key, value);
        }
    }
}
