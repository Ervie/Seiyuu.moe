using Autofac;
using JikanDotNet;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SeiyuuMoe.JikanToDBParser
{
	public class JikanParserModule: Autofac.Module
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
