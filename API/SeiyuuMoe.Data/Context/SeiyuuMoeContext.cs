using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.Data.Context
{
	public class SeiyuuMoeContext : DbContext, ISeiyuuMoeContext
	{
		private readonly string dataSource;

		public DbSet<Seiyuu> SeiyuuSet { get; set; }
		public DbSet<Anime> AnimeSet { get; set; }

		public SeiyuuMoeContext()
		{
		}

		public SeiyuuMoeContext(string dataSource)
		{
			this.dataSource = dataSource;
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = this.dataSource ?? string.Empty };
			var connectionString = connectionStringBuilder.ToString();
			var connection = new SqliteConnection(connectionString);

			optionsBuilder.UseSqlite(connection);
		}

		public void SetAttached<TEntity>(TEntity entity) where TEntity : class
		{
			if (Entry(entity).State == EntityState.Detached)
			{
				Set<TEntity>().Attach(entity);
			}
		}

		public void SetModified<TEntity>(TEntity entity) where TEntity : class
		{
			Entry(entity).State = EntityState.Modified;
		}

		public void SetDeleted<TEntity>(TEntity entity) where TEntity : class
		{
			Entry(entity).State = EntityState.Deleted;
		}
	}
}
