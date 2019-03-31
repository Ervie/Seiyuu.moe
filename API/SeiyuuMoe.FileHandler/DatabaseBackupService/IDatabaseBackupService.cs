using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeiyuuMoe.FileHandler.DatabaseBackupService
{
	public interface IDatabaseBackupService
	{
		void BackupDatabase();

		void RestoreDatabase();
	}
}
