using Autofac;
using JikanDotNet;
using System.Reflection;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParserModule : Autofac.Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<Jikan>()
				.As<IJikan>();

			builder.RegisterType<JikanParser>()
				.As<IJikanParser>();

			base.Load(builder);
		}
	}
}