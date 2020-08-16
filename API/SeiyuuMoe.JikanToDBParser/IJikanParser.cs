
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public interface IJikanParser
	{
		Task InsertNewSeiyuuAsync();

		Task ParseRolesAsync();

		Task UpdateAllCharactersAsync();

		Task UpdateAllAnimeAsync();

		Task UpdateSeasonsAsync();

		Task UpdateAllSeiyuuAsync();

		Task InsertOldSeiyuuAsync();
	}
}
