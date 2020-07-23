using Autofac;
using SeiyuuMoe.FileHandler.DatabaseBackupService;

namespace SeiyuuMoe.FileHandler
{
	public class FileHandlerModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<DatabaseBackupService.DatabaseBackupService>()
				.As<IDatabaseBackupService>();

			base.Load(builder);
		}
	}
}