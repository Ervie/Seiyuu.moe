using Autofac;
using System.Reflection;

namespace SeiyuuMoe.Infrastructure
{
	public class DomainModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterAssemblyTypes(module)
				.Where(t => t.Name.EndsWith("Repository"))
				.AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}