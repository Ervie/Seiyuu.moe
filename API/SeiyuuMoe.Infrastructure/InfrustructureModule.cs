using Autofac;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Logger;
using System.Reflection;

namespace SeiyuuMoe.Infrastructure
{
	public class InfrastructureModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<LoggingService>()
				.As<ILoggingService>();

			builder.RegisterType<SeiyuuMoeContext>().AsSelf();

			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterAssemblyTypes(module)
				.Where(t => t.Name.EndsWith("Repository"))
				.AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}