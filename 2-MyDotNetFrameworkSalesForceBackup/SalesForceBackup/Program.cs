using SalesForceBackup.Interfaces;
using SalesForceBackup.IoC;
using System;
using System.Threading.Tasks;
using TinyIoC;

namespace SalesForceBackup
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Bootstrap.Register();

                Console.WriteLine("Starting backup...");
                using (var backup = TinyIoCContainer.Current.Resolve<Backup>())
                {
                    await backup.Run(args);
                }

                Environment.Exit((int)ExitCode.Normal);
            }
            catch (Exception ex)
            {
                var _errorHandler = TinyIoCContainer.Current.Resolve<IErrorHandler>();
                _errorHandler.HandleError(ex);
                Environment.Exit((int)ExitCode.Unknown);
            }
        }
    }
}
