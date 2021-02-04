using Autofac;
using SeiyuuMoe.Infrastructure.Database.Context;
using System.Reflection;

namespace SeiyuuMoe.Infrastructure.Database
{
	public class DatabaseModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SeiyuuMoeContext>().AsSelf();

			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterAssemblyTypes(module)
				.Where(t => t.Name.EndsWith("Repository"))
				.AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}