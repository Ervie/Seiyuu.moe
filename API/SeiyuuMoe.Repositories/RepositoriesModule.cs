using Autofac;
using SeiyuuMoe.Data;
using System.Linq;
using System.Reflection;

namespace SeiyuuMoe.Repositories
{
	public class RepositoriesModule : Autofac.Module
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
