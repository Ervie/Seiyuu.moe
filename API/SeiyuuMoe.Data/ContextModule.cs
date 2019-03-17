using Autofac;
using SeiyuuMoe.Data.Context;
using System.Reflection;

namespace SeiyuuMoe.Data
{
	public class ContextModule : Autofac.Module
	{
		private readonly string pathToDB;

		public ContextModule(string dataSource)
		{
			this.pathToDB = dataSource;
		}

		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<SeiyuuMoeContext>().As<ISeiyuuMoeContext>()
				.WithParameter("dataSource", pathToDB);

			base.Load(builder);
		}
	}
}