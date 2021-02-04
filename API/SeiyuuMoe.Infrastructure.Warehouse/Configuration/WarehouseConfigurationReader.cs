using SeiyuuMoe.Infrastructure.Configuration;

namespace SeiyuuMoe.Infrastructure.Warehouse.Configuration
{
	public static class WarehouseConfigurationReader
	{
		public static WarehouseDatabaseConfiguration GetWarehouseConfiguration() => ConfigurationReader.ReturnConfigSection<WarehouseDatabaseConfiguration>("WarehouseDatabaseConfiguration");
	}
}