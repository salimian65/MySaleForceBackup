
using SalesForceBackup;
using Serilog;
using System.Net;
using MySaleForceBackup.Interfaces;

namespace MySaleForceBackup
{
    internal class Bootstrap
    {
        private readonly IConfiguration configuration;

        private readonly IServiceCollection services;

        private IEnumerable<IPEndPoint> aeroSpikeCluster;

        private readonly string brokerDatabaseConnection;

        public Bootstrap(IServiceCollection services, IConfiguration configuration)
        {
            Log.Information("2");
            this.services = services;
            this.configuration = configuration;
            brokerDatabaseConnection = configuration.GetConnectionString("Mehrdad");

        }

        public void WireUp()
        {
           
            services.AddTransient<IMyLogic, MyLogic>(); 
            services.AddScoped<IErrorHandler, ConsoleErrorHandler>();
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddScoped<IUploader>(String.Equals(System.Configuration.ConfigurationManager.AppSettings[AppSettingKeys.Uploader], Uploaders.Aws,
                StringComparison.CurrentCultureIgnoreCase)
                ? (IUploader)new S3Uploader()
                : new AzureBlobUploader());
            services.AddScoped<IDownloader>(new SalesForceWebDownloader());

            services.AddScoped<Backup>();
            Log.Information("Wire up is finished");
        }
    }
}