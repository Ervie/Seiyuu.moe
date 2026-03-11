using System;
using Microsoft.Extensions.Configuration;

namespace SeiyuuMoe.Infrastructure.Configuration
{
	public static class ConfigurationReader
	{
		private static readonly IConfigurationRoot Config = BuildConfiguration();

		private static IConfigurationRoot BuildConfiguration()
		{
			var stackName = Environment.GetEnvironmentVariable("StackName");

			if (string.IsNullOrWhiteSpace(stackName))
			{
				throw new InvalidOperationException("Environment variable 'StackName' must be set to load configuration from AWS Systems Manager.");
			}

			var configurationBuilder = new ConfigurationBuilder()
				.AddSystemsManager(config =>
				{
					config.Path = $"/{stackName}";
					config.ReloadAfter = TimeSpan.FromMinutes(5);
				});

			return configurationBuilder.Build();
		}

		public static DatabaseConfiguration DatabaseConfiguration
			=> Config
				.GetRequiredSection("DatabaseConfiguration")
				.Get<DatabaseConfiguration>()
				?? throw new InvalidOperationException("Configuration section 'DatabaseConfiguration' is missing or invalid.");

		public static MalBgJobsScheduleConfiguration MalBgJobsScheduleConfiguration
			=> Config
				.GetRequiredSection("ScheduleConfiguration")
				.Get<MalBgJobsScheduleConfiguration>()
				?? throw new InvalidOperationException("Configuration section 'ScheduleConfiguration' is missing or invalid.");

		public static string JikanUrl
			=> Config["JikanUrl"]
				?? throw new InvalidOperationException("Configuration value 'JikanUrl' is missing.");
	}
}