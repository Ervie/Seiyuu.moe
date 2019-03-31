
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public interface IJikanParser
	{
		Task InsertNewSeiyuu();

		Task ParseRoles();

		Task UpdateAllCharacters();

		Task UpdateAllAnime();

		Task UpdateSeasons();

		Task UpdateAllSeiyuu();

		Task InsertOldSeiyuu();
	}
}
