using Autofac;
using Autofac.Core;
using JikanDotNet;
using System.Reflection;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParserModule : Autofac.Module
	{
		private readonly string endpointUrl;

		public JikanParserModule(string endpointUrl)
		{
			this.endpointUrl = endpointUrl;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<JikanParser>()
				.As<IJikanParser>();

			base.Load(builder);
		}
	}
}