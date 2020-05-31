using Autofac;
using SeiyuuMoe.FileHandler.DatabaseBackupService;
using System.Reflection;

namespace SeiyuuMoe.FileHandler
{
	public class FileHandlerModule : Autofac.Module
	{
		private readonly string pathToDb;
		private readonly string pathToBackupDb;

		public FileHandlerModule(string pathToDb, string pathToBackupDb)
		{
			this.pathToDb = pathToDb;
			this.pathToBackupDb = pathToBackupDb;
		}

		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<DatabaseBackupService.DatabaseBackupService>()
				.As<IDatabaseBackupService>()
				.WithParameter("pathToDB", pathToDb)
				.WithParameter("pathToBackupDB", pathToBackupDb);

			base.Load(builder);
		}
	}
}