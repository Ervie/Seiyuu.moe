using Autofac;
using SeiyuuMoe.FileHandler.DatabaseBackupService;
using System.Reflection;

namespace SeiyuuMoe.FileHandler
{
	public class FileHandlerModule : Autofac.Module
	{
		private readonly string pathToDB;
		private readonly string pathToBackupDB;

		public FileHandlerModule(string pathToDB, string pathToBackupDB)
		{
			this.pathToDB = pathToDB;
			this.pathToBackupDB = pathToBackupDB;
		}

		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<DatabaseBackupService.DatabaseBackupService>()
				.As<IDatabaseBackupService>()
				.WithParameter("pathToDB", pathToDB)
				.WithParameter("pathToBackupDB", pathToBackupDB);

			base.Load(builder);
		}
	}
}