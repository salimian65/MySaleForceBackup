using System;
using SalesForceBackup.Interfaces;

namespace SalesForceBackup
{
    /// <summary>
    /// Outputs all errors to the console.
    /// </summary>
    public class ConsoleErrorHandler : IErrorHandler
    {


        public void HandleError(Exception e)
        {
            HandleError(e, (int)Enums.ExitCode.Unknown);
        }


        public void HandleError(Exception e, int exitCode)
        {
            HandleError(e, exitCode, "Unknown error occured:");
        }


        public void HandleError(Exception e, int exitCode, string errorMessage)
        {
            Console.WriteLine(errorMessage);
            Console.WriteLine(e.ToString());
            //   Environment.Exit(exitCode);
        }
    }
}
