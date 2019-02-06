using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.Data
{
	public class DatabaseContext : DbContext
	{
		public DbSet<Seiyuu> Seiyuus { get; set; }
		public DbSet<AnimeSnippet> Anime { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = "SeiyuuMoeDB.db" };
			var connectionString = connectionStringBuilder.ToString();
			var connection = new SqliteConnection(connectionString);

			optionsBuilder.UseSqlite(connection);
		}
	}
}