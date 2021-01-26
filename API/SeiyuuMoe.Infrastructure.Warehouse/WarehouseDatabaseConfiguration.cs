namespace SeiyuuMoe.Infrastructure.Warehouse
{
	public class WarehouseDatabaseConfiguration
	{
		public string DbHost { get; set; }
		public string DbPort { get; set; }
		public string DbName { get; set; }
		public string DbUser { get; set; }
		public string DbPass { get; set; }
		public string DbSchema { get; set; }

		public string ConnectionString => $"Host={DbHost};Port={DbPort};Database={DbName};Username={DbUser};Password={DbPass}";
	}
}