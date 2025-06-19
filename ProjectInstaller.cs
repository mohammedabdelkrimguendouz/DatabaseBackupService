using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace Database_Backup_Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        private ServiceProcessInstaller serviceProcessInstaller;
        private ServiceInstaller serviceInstaller;
        public ProjectInstaller()
        {
            InitializeComponent();

            serviceProcessInstaller = new ServiceProcessInstaller()
            {
                Account = ServiceAccount.LocalSystem,

            };

            serviceInstaller = new ServiceInstaller()
            {
                StartType = ServiceStartMode.Automatic,
                ServiceName = "DatabaseBackupService",
                DisplayName = "Database Backup Service",
                Description = "Manages automated backups of SQL Server databases, ensuring regular data protection and recovery readiness.",
                ServicesDependedOn = new string[] { "MSSQLSERVER", "EventLog", "RpcSs" }


            };

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
