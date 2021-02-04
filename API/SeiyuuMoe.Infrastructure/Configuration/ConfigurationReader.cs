using Microsoft.Extensions.Configuration;
using System;

namespace SeiyuuMoe.Infrastructure.Configuration
{
	public static class ConfigurationReader
	{
		private static readonly IConfigurationRoot Config;

		static ConfigurationReader()
		{
			var environmentType = Environment.GetEnvironmentVariable("EnvironmentType");

			var configurationBuilder = new ConfigurationBuilder()
							.AddSystemsManager(config =>
							{
								config.Path = $"/seiyuu-moe-{environmentType}";
								config.ReloadAfter = TimeSpan.FromMinutes(5);
							});

			Config = configurationBuilder.Build();
		}

		public static MalBgJobsScheduleConfiguration MalBgJobsScheduleConfiguration
			=> ReturnConfigSection<MalBgJobsScheduleConfiguration>("ScheduleConfiguration");

		public static string JikanUrl => Config["JikanUrl"];

		public static T ReturnConfigSection<T>(string sectionName) where T : new()
		{
			var sectionObject = new T();
			Config.GetSection(sectionName).Bind(sectionObject);
			return sectionObject;
		}
	}
}