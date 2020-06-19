namespace SeiyuuMoe.FileHandler.DatabaseBackupService
{
	public interface IDatabaseBackupService
	{
		void BackupDatabase();

		void RestoreDatabase();
	}
}
