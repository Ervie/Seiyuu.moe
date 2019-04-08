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
			var module = Assembly.GetAssembly(this.GetType());

			builder.RegisterType<JikanParser>()
				.As<IJikanParser>()
				.WithParameter("endpointUrl", endpointUrl);

			base.Load(builder);
		}
	}
}