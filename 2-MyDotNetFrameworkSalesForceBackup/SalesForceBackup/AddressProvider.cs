using System;
using System.Globalization;
using System.IO;
using SalesForceBackup.Interfaces;

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

    }
}
