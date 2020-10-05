using Microsoft.EntityFrameworkCore.Design;
using SeiyuuMoe.Infrastructure.Configuration;
using System;

namespace SeiyuuMoe.Infrastructure.Context
{
	public class SeiyuuMoeContextFactory : IDesignTimeDbContextFactory<SeiyuuMoeContext>
	{
		public SeiyuuMoeContext CreateDbContext(string[] args)
		{
			Environment.SetEnvironmentVariable("StackName", "seiyuu-moe-mal-bg-jobs-dev");

			var databaseConfiguration = ConfigurationReader.DatabaseConfiguration;

			return new SeiyuuMoeContext(databaseConfiguration);
		}
	}
}