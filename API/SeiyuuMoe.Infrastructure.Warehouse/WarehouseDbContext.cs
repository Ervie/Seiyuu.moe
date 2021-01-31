using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System;

namespace SeiyuuMoe.Infrastructure.Warehouse
{
	public class WarehouseDbContext : DbContext
	{
		private readonly WarehouseDatabaseConfiguration _databaseConfiguration;

		public DbSet<VndbVisualNovel> VisualNovels { get; set; }
		public DbSet<VndbCharacter> Characters { get; set; }
		public DbSet<VndbCharacterVisualNovel> CharacterVisualNovels { get; set; }
		public DbSet<VndbStaff> Staffs { get; set; }
		public DbSet<VndbStaffAlias> StaffAliases { get; set; }
		public DbSet<VndbVisualNovelSeiyuu> VisualNovelSeiyuus { get; set; }

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


			modelBuilder.Entity<VndbCharacterVisualNovel>(entity =>
			{
				entity.HasNoKey();

				//entity.HasOne(d => d.Character)
				//	.WithMany(p => p.CharacterVisualNovels)
				//	.HasForeignKey(d => d.CharacterId)
				//	.OnDelete(DeleteBehavior.Cascade);

				//entity.HasOne(d => d.VisualNovel)
				//	.WithMany(p => p.CharacterVisualNovels)
				//	.HasForeignKey(d => d.VisualNovelId)
				//	.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<VndbVisualNovelSeiyuu>().HasKey(p => new { p.VisualNovelId, p.StaffId, p.CharacterId });
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