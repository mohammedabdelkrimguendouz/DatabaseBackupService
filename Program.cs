using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Database_Backup_Service
{
    internal static class Program
    {
        /// <summary>
        /// Main entry point for the application
        /// </summary>
        static void Main()
        {
            if (Environment.UserInteractive) 
            {
                // Run the service in console mode
                Console.WriteLine("Running in console mode...");
                DatabaseBackupService service = new DatabaseBackupService();
                service.StartInConsole();
            }
            else
            {
                // Run the service in normal mode
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new DatabaseBackupService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
