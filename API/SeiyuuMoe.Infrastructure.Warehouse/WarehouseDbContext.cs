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
			});

			modelBuilder.Entity<VndbVisualNovelSeiyuu>().HasKey(p => new { p.VisualNovelId, p.StaffAliasId, p.CharacterId });

			modelBuilder.Entity<VndbStaff>()
				.HasOne(x => x.MainAlias)
				.WithOne(x => x.MainStaff)
				.HasConstraintName("staff_aid_fkey");

			modelBuilder.Entity<VndbStaff>()
				.HasMany(x => x.Aliases)
				.WithOne(x => x.Staff)
				.HasForeignKey(x => x.Id)
				.HasConstraintName("staff_alias_id_fkey");
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