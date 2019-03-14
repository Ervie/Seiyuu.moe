using Autofac;
using System.Reflection;

namespace SeiyuuMoe.Logger
{
	public class LoggerModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<LoggingService>()
				.As<ILoggingService>();

			base.Load(builder);
		}
	}
}