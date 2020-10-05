using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Configuration;

namespace SeiyuuMoe.Infrastructure.Context
{
	public partial class SeiyuuMoeContext : DbContext
	{
		private readonly DatabaseConfiguration _databaseConfiguration;

		public SeiyuuMoeContext(DbContextOptions<SeiyuuMoeContext> options)
			: base(options)
		{
		}

		public SeiyuuMoeContext(DatabaseConfiguration configuration)
		{
			_databaseConfiguration = configuration;
		}

		public virtual DbSet<Anime> Animes { get; set; }
		public virtual DbSet<AnimeStatus> AnimeStatuses { get; set; }
		public virtual DbSet<AnimeType> AnimeTypes { get; set; }
		public virtual DbSet<Blacklist> Blacklists { get; set; }
		public virtual DbSet<AnimeCharacter> AnimeCharacters { get; set; }
		public virtual DbSet<Language> Languages { get; set; }
		public virtual DbSet<AnimeRole> AnimeRoles { get; set; }
		public virtual DbSet<AnimeRoleType> AnimeRoleTypes { get; set; }
		public virtual DbSet<AnimeSeason> AnimeSeasons { get; set; }
		public virtual DbSet<Seiyuu> Seiyuus { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (optionsBuilder.IsConfigured)
			{
				return;
			}
			optionsBuilder.UseMySql(_databaseConfiguration.ToConnectionString);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "3.0.0-preview.19074.3");

			modelBuilder.Entity<Anime>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(uuid())");

				entity.HasIndex(e => e.MalId).IsUnique();

				entity.Property(e => e.AiringDate).HasColumnType("DATE");

				entity.Property(e => e.Title).IsRequired();

				entity.HasOne(d => d.Season)
					.WithMany(p => p.Anime)
					.HasForeignKey(d => d.SeasonId)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Status)
					.WithMany(p => p.Anime)
					.HasForeignKey(d => d.StatusId)
					.OnDelete(DeleteBehavior.SetNull);

				entity.HasOne(d => d.Type)
					.WithMany(p => p.Anime)
					.HasForeignKey(d => d.TypeId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			modelBuilder.Entity<AnimeCharacter>(entity =>
			{
				entity.HasIndex(e => e.MalId).IsUnique();
			});

			modelBuilder.Entity<AnimeStatus>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Description).IsRequired();
			});

			modelBuilder.Entity<AnimeType>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Description).IsRequired();
			});

			modelBuilder.Entity<Blacklist>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(uuid())");

				entity.Property(e => e.MalId).IsRequired();

				entity.Property(e => e.EntityType).IsRequired();
			});

			modelBuilder.Entity<AnimeCharacter>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(uuid())");

				entity.HasIndex(e => e.MalId).IsUnique();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<Language>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Description).IsRequired();
			});

			modelBuilder.Entity<AnimeRole>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(uuid())");

				entity.HasOne(d => d.Anime)
					.WithMany(p => p.Role)
					.HasForeignKey(d => d.AnimeId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(d => d.Character)
					.WithMany(p => p.Role)
					.HasForeignKey(d => d.CharacterId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(d => d.RoleType)
					.WithMany(p => p.Role)
					.HasForeignKey(d => d.RoleTypeId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(d => d.Seiyuu)
					.WithMany(p => p.Role)
					.HasForeignKey(d => d.SeiyuuId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<AnimeRoleType>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Description).IsRequired();
			});

			modelBuilder.Entity<AnimeSeason>(entity =>
			{
				entity.Property(e => e.Year).IsRequired();
				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<Seiyuu>(entity =>
			{
				entity.Property(e => e.Id).HasDefaultValueSql("(uuid())");
				entity.HasIndex(e => e.MalId).IsUnique();
				entity.Property(e => e.Birthday).HasColumnType("DATE");

				entity.Property(e => e.Name).IsRequired();
			});
		}
	}
}