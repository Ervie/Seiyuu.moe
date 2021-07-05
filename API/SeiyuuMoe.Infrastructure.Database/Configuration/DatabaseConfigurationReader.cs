using SeiyuuMoe.Infrastructure.Configuration;

namespace SeiyuuMoe.Infrastructure.Database.Configuration
{
	public static class DatabaseConfigurationReader
	{
		public static DatabaseConfiguration DatabaseConfiguration => ConfigurationReader.DatabaseConfiguration;
	}
}