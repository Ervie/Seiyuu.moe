using SeiyuuMoe.Infrastructure.Configuration;

namespace SeiyuuMoe.Infrastructure.Database.Configuration
{
	public static class DatabaseConfigurationReader
	{
		public static DatabaseConfiguration GetDatabaseConfiguration() => ConfigurationReader.ReturnConfigSection<DatabaseConfiguration>("DatabaseConfiguration");
	}
}