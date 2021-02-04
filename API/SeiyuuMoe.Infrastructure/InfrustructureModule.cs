using Autofac;
using SeiyuuMoe.Infrastructure.Logger;

namespace SeiyuuMoe.Infrastructure
{
	public class InfrastructureModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<LoggingService>()
				.As<ILoggingService>();

			base.Load(builder);
		}
	}
}