using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using SalesForceBackup.Interfaces;
using TinyIoC;

namespace SalesForceBackup
{

    public class AppSettings : IAppSettings
    {
        private readonly IErrorHandler _errorHandler;
        private readonly Dictionary<string, string> ValuePairs;
        public int age { get; set; }

        public AppSettings(IErrorHandler errorHandler)
        {
            //  _errorHandler = TinyIoCContainer.Current.Resolve<IErrorHandler>();
            _errorHandler = errorHandler;
            ValuePairs = new Dictionary<string, string>();
            ReadAllSettings();
        }


        public void AssignValues(IList<string> args)
        {
            for (var i = 0; i < args.Count(); i++)
            {
                switch (args[i])
                {
                    case "--help":
                        DisplayHelp();
                        Environment.Exit((int)Enums.ExitCode.Normal);
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

        public string Get(string key)
        {
            string value;
            var IsSuccuessful = ValuePairs.TryGetValue(key, out value);
            IsSuccuessfulGetValue(IsSuccuessful, key);
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

        private void ReadAllSettings()
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;

                if (appSettings.Count == 0) return;
                foreach (var key in appSettings.AllKeys)
                {
                    ValuePairs.Add(key, appSettings[key]);
                }
            }
            catch (ConfigurationErrorsException e)
            {
                Console.WriteLine("Error reading app settings");
                _errorHandler.HandleError(e, (int)Enums.ExitCode.ConfigurationError);
            }

        }

        private  void DisplayHelp()
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

        private void IsSuccuessfulGetValue(bool IsSuccuessful, string key)
        {
            if (!IsSuccuessful)
            {
                throw new ConfigurationErrorsException(String.Format("AppSettings cannot find corresponding value for {0} key", key));
            }
        }
    }
}
