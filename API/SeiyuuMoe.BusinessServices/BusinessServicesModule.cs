using Autofac;
using System.Linq;

namespace SeiyuuMoe.BusinessServices
{
	public class BusinessServicesModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(this.ThisAssembly)
				.Where(t => t.Name.EndsWith("Service"))
				.AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}