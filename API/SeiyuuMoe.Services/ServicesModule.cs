using Autofac;

namespace SeiyuuMoe.Services
{
	public class ServicesModule : Module
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