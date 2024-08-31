using System.Diagnostics;

namespace Call_MySaleForceBackup
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            using (Process process = new Process())
            {
                process.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                process.StartInfo.Arguments = "-u 1 -p 1 -t 1 -h 1 -a 1 -y 1 -z 1 -s 1";
                process.StartInfo.FileName =
                    "MySaleForceBackup.exe";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();

                // Synchronously read the standard output of the spawned process.
                StreamReader reader = process.StandardOutput;
                string output = reader.ReadToEnd();

                // Write the redirected output to this application's window.
                Console.WriteLine(output);

                process.WaitForExit();
            }

            Console.WriteLine("\n\nPress any key to exit.");
            Console.ReadLine();
        }
    }
}
