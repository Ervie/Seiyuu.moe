using Autofac;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParserModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<JikanParser>()
				.As<IJikanParser>();

			base.Load(builder);
		}
	}
}