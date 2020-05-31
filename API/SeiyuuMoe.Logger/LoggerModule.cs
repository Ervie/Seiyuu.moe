using Autofac;
namespace SeiyuuMoe.Logger
{
	public class LoggerModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<LoggingService>()
				.As<ILoggingService>();

			base.Load(builder);
		}
	}
}