using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Context;
using System;

namespace SeiyuuMoe.Tests.Unit.Helpers
{
	internal static class InMemoryDbProvider
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
	}
}