using Autofac;
namespace SeiyuuMoe.Logger
{
	public class InfrastructureModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<LoggingService>()
				.As<ILoggingService>();

			base.Load(builder);
		}
	}
}