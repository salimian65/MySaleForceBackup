using Microsoft.Extensions.DependencyInjection;
using MySaleForceBackup.Interfaces;

namespace MySaleForceBackup
{
    class MyApp
    {
        private readonly ILogger<MyApp> _logger;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppSettings _appSettings;

        public MyApp(ILogger<MyApp> logger, IConfiguration config, AppSettings appSettings, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _config = config;
            _appSettings = appSettings;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(IList<string> args)
        {
            _appSettings.FillSetting(args);
            _logger.LogInformation(_config["App:Value1"]);
            using var scopePerRequest = _serviceProvider.CreateScope();
            var backupProcess = scopePerRequest.ServiceProvider.GetService<Backup>();
            backupProcess?.Run();
            return Task.CompletedTask;
        }
    }

}