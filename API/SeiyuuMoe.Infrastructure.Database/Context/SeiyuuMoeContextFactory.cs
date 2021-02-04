using Microsoft.EntityFrameworkCore.Design;
using SeiyuuMoe.Infrastructure.Database.Configuration;
using System;

namespace SeiyuuMoe.Infrastructure.Database.Context
{
	public class SeiyuuMoeContextFactory : IDesignTimeDbContextFactory<SeiyuuMoeContext>
	{
		public SeiyuuMoeContext CreateDbContext(string[] args)
		{
			Environment.SetEnvironmentVariable("EnvironmentType", "dev");

			var databaseConfiguration = DatabaseConfigurationReader.GetDatabaseConfiguration();

			return new SeiyuuMoeContext(databaseConfiguration);
		}
	}
}