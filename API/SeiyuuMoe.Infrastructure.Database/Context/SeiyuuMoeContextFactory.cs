using Microsoft.EntityFrameworkCore.Design;
using SeiyuuMoe.Infrastructure.Database.Configuration;
using System;

namespace SeiyuuMoe.Infrastructure.Database.Context
{
	public class SeiyuuMoeContextFactory : IDesignTimeDbContextFactory<SeiyuuMoeContext>
	{
		public SeiyuuMoeContext CreateDbContext(string[] args)
		{
			Environment.SetEnvironmentVariable("StackName", "seiyuu-moe-mal-bg-jobs-dev");

			var databaseConfiguration = DatabaseConfigurationReader.DatabaseConfiguration;

			return new SeiyuuMoeContext(databaseConfiguration);
		}
	}
}