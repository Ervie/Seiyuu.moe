﻿using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Configuration;

namespace SeiyuuMoe.Infrastructure.Context
{
	public partial class SeiyuuMoeContext : DbContext
	{
		private readonly string dataSource;

		public SeiyuuMoeContext()
		{
		}

		public SeiyuuMoeContext(DbContextOptions<SeiyuuMoeContext> options)
			: base(options)
		{
		}

		public SeiyuuMoeContext(SeiyuuMoeConfiguration configuration)
		{
			dataSource = configuration.PathToDB;
		}

		public virtual DbSet<Anime> Anime { get; set; }
		public virtual DbSet<AnimeStatus> AnimeStatus { get; set; }
		public virtual DbSet<AnimeType> AnimeType { get; set; }
		public virtual DbSet<BlacklistedId> BlacklistedId { get; set; }
		public virtual DbSet<Character> Character { get; set; }
		public virtual DbSet<Language> Language { get; set; }
		public virtual DbSet<Role> Role { get; set; }
		public virtual DbSet<RoleType> RoleType { get; set; }
		public virtual DbSet<Domain.Entities.Season> Season { get; set; }
		public virtual DbSet<Domain.Entities.Seiyuu> Seiyuu { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = this.dataSource ?? string.Empty };
				var connectionString = connectionStringBuilder.ToString();
				var connection = new SqliteConnection(connectionString);

				optionsBuilder.UseSqlite(connection);
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.HasAnnotation("ProductVersion", "3.0.0-preview.19074.3");

			modelBuilder.Entity<Anime>(entity =>
			{
				entity.Property(e => e.AiringDate).HasColumnType("NUMERIC");

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

			modelBuilder.Entity<AnimeStatus>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<AnimeType>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<BlacklistedId>(entity =>
			{
				//entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.MalId).IsRequired();

				entity.Property(e => e.EntityType).IsRequired();
			});

			modelBuilder.Entity<Character>(entity =>
			{
				//entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<Language>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<Role>(entity =>
			{
				//entity.Property(e => e.Id).ValueGeneratedNever();

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

			modelBuilder.Entity<RoleType>(entity =>
			{
				entity.Property(e => e.Id).ValueGeneratedNever();

				entity.Property(e => e.Name).IsRequired();
			});

			modelBuilder.Entity<Domain.Entities.Season>(entity =>
			{
				//entity.Property(e => e.Id).ValueGeneratedNever();
			});

			modelBuilder.Entity<Domain.Entities.Seiyuu>(entity =>
			{
				entity.Property(e => e.Birthday).HasColumnType("NUMERIC");

				entity.Property(e => e.Name).IsRequired();
			});
		}
	}
}