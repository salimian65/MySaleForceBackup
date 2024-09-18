using Serilog;
using MySaleForceBackup.Interfaces;
using System.Threading.Channels;

namespace MySaleForceBackup
{
    internal class Bootstrap
    {
        private readonly IConfiguration configuration;
        private readonly string[] args;
        private readonly IServiceCollection services;
  

        public Bootstrap(IServiceCollection services, IConfiguration configuration, string[] args)
        {
            Log.Information("Bootstrap starting ...");
            this.services = services;
            this.configuration = configuration;
            this.args = args;
            this.args = configuration.GetValue<string[]>("args");
        }

        public void WireUp()
        {
            services.AddTransient<IErrorHandler, ConsoleErrorHandler>();
            services.AddTransient<IDownloader, SalesForceWebDownloader>();
            services.AddTransient<Backup>();
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddSingleton<MyApp>();
            services.AddHttpClient();
            services.AddSingleton(Channel.CreateUnbounded<string>());
            var appSettingConfig = System.Configuration.ConfigurationManager.AppSettings;
            var isAws = String.Equals(appSettingConfig[AppSettingKeys.Uploader], Uploaders.Aws, StringComparison.CurrentCultureIgnoreCase);
            if (isAws)
            {
                services.AddScoped<IUploader, S3Uploader>();
            }
            else
            {
                services.AddScoped<IUploader, AzureBlobUploader>();
            }
            Log.Information("Wire up is finished");
        }

    }
}

// Registration.RegisterAllImplementationOfInterface<IUploader>(services);
//services.AddScoped<IUploader>(String.Equals(System.Configuration.ConfigurationManager.AppSettings[AppSettingKeys.Uploader], Uploaders.Aws,
//    StringComparison.CurrentCultureIgnoreCase)
//    ? (IUploader)new S3Uploader()
//    : new AzureBlobUploader());