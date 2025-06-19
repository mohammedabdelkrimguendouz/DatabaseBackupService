using System;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Configuration;
using System.Data.SqlClient;

namespace Database_Backup_Service
{
    public partial class DatabaseBackupService : ServiceBase
    {
        private  Timer backupTimer; // Timer for scheduling periodic backups

        // Service configuration variables
        private string ConnectionString;  
        private string BackupFolder;      
        private string LogFolder;          
        private int BackupIntervalMinutes; 

        public DatabaseBackupService()
        {
            InitializeComponent();

            // Read configuration values from App.config
            ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
            BackupFolder = ConfigurationManager.AppSettings["BackupFolder"];
            LogFolder = ConfigurationManager.AppSettings["LogFolder"];

            string BackupIntervalMinutesAsString = ConfigurationManager.AppSettings["BackupIntervalMinutes"];

            // Check if BackupIntervalMinutes is missing or invalid
            if (string.IsNullOrWhiteSpace(BackupIntervalMinutesAsString))
            {
                BackupIntervalMinutes = 60; // Default to 60 minutes
                Log("BackupIntervalMinutes is missing in App.config. Using default: " + BackupIntervalMinutes);
            }
            else if (!IsNumber(BackupIntervalMinutesAsString))
            {
                BackupIntervalMinutes = 60; // Default to 60 minutes
                Log("BackupIntervalMinutes is an invalid number. Using default: " + BackupIntervalMinutes);
            }
            else
            {
                BackupIntervalMinutes = Convert.ToInt32(BackupIntervalMinutesAsString);
            }

            
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                // Use default connection string if not provided
                ConnectionString = @"Server=.;Database=StudentsDB;User Id=sa;Password=sa123456;Integrated Security=True;";
                Log("ConnectionString is missing in App.config. Using default: " + ConnectionString);
            }

           
            if (string.IsNullOrWhiteSpace(BackupFolder))
            {
                BackupFolder = @"C:\DatabaseBackups"; // Default backup folder
                Log("BackupFolder is missing in App.config. Using default: " + BackupFolder);
            }

           
            if (string.IsNullOrWhiteSpace(LogFolder))
            {
                LogFolder = @"C:\DatabaseBackups\Logs"; // Default log folder
                Log("LogFolder is missing in App.config. Using default: " + LogFolder);
            }

           
            Directory.CreateDirectory(BackupFolder);
            Directory.CreateDirectory(LogFolder);
        }

     
        private bool IsNumber(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                    return false;
            }
            return true;
        }

        private void Log(string Message)
        {
            string LogFilePath = Path.Combine(LogFolder, "BackupDatabaseLogs.txt");
            string LogMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {Message}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, LogMessage);

            if (Environment.UserInteractive) 
                Console.WriteLine(LogMessage);
        }

        // Event handler triggered by the timer for periodic backups
        private void PerformBackup(object state)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string BackupFileName = Path.Combine(BackupFolder, $"Backup_{timestamp}.bak");

            try
            {
                // Perform database backup using a stored procedure
                using (SqlConnection Connection = new SqlConnection(ConnectionString))
                {
                    Connection.Open();
                    string Query = $@"BACKUP DATABASE [{Connection.Database}] TO DISK = '{BackupFileName}' WITH INIT";
                    using (SqlCommand Command = new SqlCommand(Query, Connection))
                    {
                        Command.ExecuteNonQuery();
                    }
                }

                Log($"Database backup successful: {BackupFileName}");
            }
            catch (Exception Ex)
            {
                Log($"Error during backup: {Ex.Message}");
            }
        }

      
        protected override void OnStart(string[] args)
        {
            Log("Service Started");


            backupTimer = new Timer(
                callback: PerformBackup,                  // Callback method
                state: null,                              // State object (not used here)
                dueTime: TimeSpan.Zero,                   // Start immediately
                period: TimeSpan.FromMinutes(BackupIntervalMinutes) // Interval
            );


            Log($"Backup schedule initiated: every {BackupIntervalMinutes} minute(s).");
        }

      
        protected override void OnStop()
        {
            backupTimer?.Dispose();       // Release timer resources
            Log("Service Stopped");
        }

        // Debugging method to run the service in console mode
        public void StartInConsole()
        {
            OnStart(null);
            Console.WriteLine("Service running in console mode. Press Enter to stop...");
            Console.ReadLine();
            OnStop();
        }
    }
}
