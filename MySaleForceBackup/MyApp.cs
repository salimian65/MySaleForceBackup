namespace MySaleForceBackup
{
    class MyApp
    {
        private ILogger<MyApp> _logger;
        private IConfiguration _config;
        private IMyLogic _logic;

        public MyApp(ILogger<MyApp> logger, IConfiguration config, IMyLogic logic)
        {
            _logger = logger;
            _config = config;
            _logic = logic;
        }

        public Task StartAsync()
        {
            _logger.LogInformation(_config["App:Value1"]);
            _logic.Say("Hello World!");
            return Task.CompletedTask;
        }
    }

    public interface IMyLogic
    {
        void Say(string message);
    }

    public class MyLogic : IMyLogic
    {
        public void Say(string message)
        {
            Console.WriteLine(message);
        }
    }
}