using Microsoft.Data.Sqlite;
using SeiyuuMoe.Logger;

namespace SeiyuuMoe.FileHandler.DatabaseBackupService
{
	internal class DatabaseBackupService : IDatabaseBackupService
	{
		private readonly ILoggingService logger;

		private readonly string pathToDb;
		private readonly string pathToBackupDb;

		public DatabaseBackupService(string pathToDb, string pathToBackupDb, ILoggingService loggingService)
		{
			this.pathToDb = pathToDb;
			this.pathToBackupDb = pathToBackupDb;
			logger = loggingService;
		}

		public void BackupDatabase()
		{
			logger.Log("Started BackupDatabase job.");

			using (var source = new SqliteConnection($"Data Source={pathToDb};"))
			using (var destination = new SqliteConnection($"Data Source={pathToBackupDb};"))
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

			using (var source = new SqliteConnection($"Data Source={pathToBackupDb};"))
			using (var destination = new SqliteConnection($"Data Source={pathToDb};"))
			{
				source.Open();
				destination.Open();
				source.BackupDatabase(destination);
			}

			logger.Log("Finished RestoreDatabase job.");
		}
	}
}