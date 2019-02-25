using Autofac;
using AutoMapper;
using SeiyuuMoe.BusinessServices.Mapper;
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

			builder.RegisterAssemblyTypes(ThisAssembly)
				.Where(t => t.Name.EndsWith("Profile"))
				.As<Profile>();

			builder.RegisterType<AutoMapperConfiguration>().As<IConfigurationProvider>();
			builder.RegisterType<AutoMapper.Mapper>().As<IMapper>();

			base.Load(builder);
		}
	}
}