using Microsoft.Extensions.Configuration;
using System;

namespace SeiyuuMoe.Infrastructure.Configuration
{
	public static class ConfigurationReader
	{
		private static readonly IConfigurationRoot Config;

		static ConfigurationReader()
		{
			var stackName = Environment.GetEnvironmentVariable("StackName");

			var configurationBuilder = new ConfigurationBuilder()
							.AddSystemsManager(config =>
							{
								config.Path = $"/{stackName}";
								config.ReloadAfter = TimeSpan.FromMinutes(5);
							});

			Config = configurationBuilder.Build();
		}

		public static DatabaseConfiguration DatabaseConfiguration
			=> ReturnConfigSection<DatabaseConfiguration>("DatabaseConfiguration");

		public static string JikanUrl => Config["JikanUrl"];

		private static T ReturnConfigSection<T>(string sectionName) where T : new()
		{
			var sectionObject = new T();
			Config.GetSection(sectionName).Bind(sectionObject);
			return sectionObject;
		}
	}
}