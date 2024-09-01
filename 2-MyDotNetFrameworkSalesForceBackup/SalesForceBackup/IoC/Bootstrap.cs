using SalesForceBackup.Interfaces;
using System;
using System.Configuration;
using TinyIoC;

namespace SalesForceBackup.IoC
{
    public static class Bootstrap
    {
        public static void Register()
        {
            TinyIoCContainer.Current.Register<IErrorHandler, ConsoleErrorHandler>();//singleton
            TinyIoCContainer.Current.Register<IAppSettings, AppSettings>();//singleton
            TinyIoCContainer.Current.Register<IDownloader, SalesForceWebDownloader>();//singleton
            TinyIoCContainer.Current.Register<IAddressProvider, AddressProvider>();//singleton
            TinyIoCContainer.Current.Register<Backup>(); //transiant

            var appSettingConfig = ConfigurationManager.AppSettings;
            var isAws = String.Equals(appSettingConfig[AppSettingKeys.Uploader], Uploaders.Aws, StringComparison.CurrentCultureIgnoreCase);
            if (isAws)
            {
                TinyIoCContainer.Current.Register<IUploader, S3Uploader>(); //singleton
            }
            else
            {
                TinyIoCContainer.Current.Register<IUploader, AzureBlobUploader>(); //singleton
            }
        }
    }
}
