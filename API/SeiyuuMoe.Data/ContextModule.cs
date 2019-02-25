using Autofac;
using SeiyuuMoe.Data.Context;
using System.Reflection;

namespace SeiyuuMoe.Data
{
	public class ContextModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<SeiyuuMoeContext>().As<ISeiyuuMoeContext>()
				.WithParameter("dataSource", "SeiyuuMoeDB.db");

			base.Load(builder);
		}
	}
}