using Microsoft.Data.Sqlite;
using SeiyuuMoe.Logger;

namespace SeiyuuMoe.FileHandler.DatabaseBackupService
{
	internal class DatabaseBackupService : IDatabaseBackupService
	{
		private ILoggingService logger;

		private readonly string pathToDB;
		private readonly string pathToBackupDB;

		public DatabaseBackupService(string pathToDB, string pathToBackupDB, ILoggingService loggingService)
		{
			this.pathToDB = pathToDB;
			this.pathToBackupDB = pathToBackupDB;
			logger = loggingService;
		}

		public void BackupDatabase()
		{
			logger.Log("Started BackupDatabase job.");

			using (var source = new SqliteConnection($"Data Source={pathToDB};"))
			using (var destination = new SqliteConnection($"Data Source={pathToBackupDB};"))
			{
				source.Open();
				destination.Open();
				source.BackupDatabase(destination);
			}

			logger.Log("Finished BackupDatabase job.");
		}

		public void RestoreDatabase()
		{
			logger.Log("Started RestoreDatabase job.");

			using (var source = new SqliteConnection($"Data Source={pathToBackupDB};"))
			using (var destination = new SqliteConnection($"Data Source={pathToDB};"))
			{
				source.Open();
				destination.Open();
				source.BackupDatabase(destination);
			}

			logger.Log("Finished RestoreDatabase job.");
		}
	}
}