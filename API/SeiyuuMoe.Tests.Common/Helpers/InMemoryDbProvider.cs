using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Warehouse;
using System;

namespace SeiyuuMoe.Tests.Common.Helpers
{
	public static class InMemoryDbProvider
	{
		public static SeiyuuMoeContext GetDbContext(string dbName = "")
		{
			var options = new DbContextOptionsBuilder<SeiyuuMoeContext>()
				.UseInMemoryDatabase(string.IsNullOrEmpty(dbName) ? Guid.NewGuid().ToString() : dbName)
				.EnableSensitiveDataLogging()
				.Options;
			var context = new SeiyuuMoeContext(options);

			return context;
		}

		public static WarehouseDbContext GetWarehouseDbContext(string dbName = "")
		{
			var options = new DbContextOptionsBuilder<WarehouseDbContext>()
				.UseInMemoryDatabase(string.IsNullOrEmpty(dbName) ? Guid.NewGuid().ToString() : dbName)
				.EnableSensitiveDataLogging()
				.Options;
			var context = new WarehouseDbContext(options);

			return context;
		}
	}
}