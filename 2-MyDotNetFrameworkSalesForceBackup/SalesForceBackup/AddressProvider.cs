using SalesForceBackup.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace SalesForceBackup
{
    public class AddressProvider : IAddressProvider
    {
        private readonly IAppSettings _appSettings;
        private readonly IErrorHandler _errorHandler;

        public AddressProvider(IAppSettings appSettings, IErrorHandler errorHandler)
        {
            _appSettings = appSettings;
            _errorHandler = errorHandler;
        }


        public Uri SalesForceBaseAddress()
        {
            return new Uri(String.Format("{0}://{1}", _appSettings.Get(AppSettingKeys.Scheme), _appSettings.Get(AppSettingKeys.Host)));
        }

        public string SalesForceSaveAddress(string fileName)
        {
            return String.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), new[] { Environment.CurrentDirectory, fileName });
        }

        public string SalesForceUrlFormater(Match match)
        {
            return String.Format("{0}{1}", _appSettings.Get(AppSettingKeys.DownloadPage), match.ToString().Replace("&amp;", "&"));
        }
    }
}
