using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System;

namespace SeiyuuMoe.Infrastructure.Warehouse
{
	internal class WarehouseDbContext : DbContext
	{
		private readonly WarehouseDatabaseConfiguration _databaseConfiguration;

		public DbSet<VndbVisualNovel> VisualNovels { get; set; }

		public WarehouseDbContext(WarehouseDatabaseConfiguration databaseConfiguration)
		{
			_databaseConfiguration = databaseConfiguration;
		}

		public WarehouseDbContext(DbContextOptions options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			var dbSchema = _databaseConfiguration?.DbSchema;

			if (!String.IsNullOrWhiteSpace(dbSchema))
			{
				modelBuilder.HasDefaultSchema(_databaseConfiguration.DbSchema);
			}
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

			if (optionsBuilder.IsConfigured)
			{
				return;
			}

			optionsBuilder.UseNpgsql(_databaseConfiguration.ConnectionString);
		}
	}
}