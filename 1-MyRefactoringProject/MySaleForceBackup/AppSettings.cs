using MySaleForceBackup.Interfaces;
using SalesForceBackup;
using System.Configuration;
using System.Text;


namespace MySaleForceBackup
{
    /// <summary>
    /// Reads the App.Config application settings.
    /// </summary>
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        private readonly IErrorHandler _errorHandler;
        private readonly Dictionary<string, string> _valuePairs = new Dictionary<string, string>();


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
            _valuePairs.TryGetValue(key, out value);
            return value;
        }


        public void Set(string key, string value)
        {
            if (_valuePairs.ContainsKey(key))
            {
                _valuePairs.Remove(key);
            }
            _valuePairs.Add(key, value);
        }

        public void FillSetting(IList<string> args)
        {
            for (var i = 0; i < args.Count(); i++)
            {
                switch (args[i])
                {
                    case "--help":
                        DisplayHelp();
                        Environment.Exit((int)ExitCode.Normal);
                        break;
                    case "-u":
                        Set(AppSettingKeys.Username, args[++i]);
                        break;
                    case "-p":
                        Set(AppSettingKeys.Password, args[++i]);
                        break;
                    case "-t":
                        Set(AppSettingKeys.SecurityToken, args[++i]);
                        break;
                    case "-h":
                        Set(AppSettingKeys.Host, args[++i]);
                        break;
                    case "-a":
                        Set(AppSettingKeys.AwsAccessKey, args[++i]);
                        break;
                    case "-y":
                        Set(AppSettingKeys.AzureAccountName, args[++i]);
                        break;
                    case "-z":
                        Set(AppSettingKeys.AzureSharedKey, args[++i]);
                        break;
                    case "-s":
                        Set(AppSettingKeys.AwsSecretKey, args[++i]);
                        break;
                }
            }
        }

        private static void DisplayHelp()
        {
            var file = AppDomain.CurrentDomain.FriendlyName;
            var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var sb = new StringBuilder(1024);
            using (var sr = new StringWriter(sb))
            {
                sr.WriteLine("{0} version {1}", name, version);
                sr.WriteLine("");
                sr.WriteLine("Usage: {0} [-hupasyz]", file);
                sr.WriteLine("");
                sr.WriteLine("Options:");
                sr.WriteLine("\t-h or --help\tDisplays this help text");
                sr.WriteLine("\t-u \t\tUsername for SalesForce");
                sr.WriteLine("\t-p \t\tPassword for SalesForce");
                sr.WriteLine("\t-a \t\tAWS access key");
                sr.WriteLine("\t-s \t\tAWS secret key");
                sr.WriteLine("\t-y \t\tAzure account name");
                sr.WriteLine("\t-z \t\tAzure shared key");
            }
            Console.Write(sb.ToString());
        }
    }
}
