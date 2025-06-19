1-"# DatabaseBackupService" : Manages automated backups of SQL Server databases, ensuring regular data protection and recovery readiness.
2-HOW IT WORKS ? 
   - This service operates periodically and automatically based on a timer.
   - You connect to a custom database in sql server and make a backup in a specific folder.
3-DESIGN DECISIONS :
   - I used C# programming language and Timer class which acts as a specific timer and executes specific code after every specified time intervals.
   - I created a log file in the log folder. It records service start and stop events, successful database backups, and errors during backup or connection.
   - The service offers flexible configurations with adjustable paths and settings in configuration files.
   - I used ADO .NET to connect and interact with a database.









