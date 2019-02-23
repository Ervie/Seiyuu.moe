using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.Data
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Seiyuu> SeiyuuSet { get; set; }
		public DbSet<Anime> AnimeSet { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "SeiyuuMoeDB.db" };
			var connectionString = connectionStringBuilder.ToString();
			var connection = new SqliteConnection(connectionString);

			optionsBuilder.UseSqlite(connection);
		}
	}
}