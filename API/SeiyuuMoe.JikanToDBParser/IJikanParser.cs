
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public interface IJikanParser
	{
		Task InsertSeiyuu();

		Task ParseRoles();

		Task UpdateAllCharacters();

		Task UpdateAllAnime();

		Task UpdateSeasons();

		Task UpdateAllSeiyuu();
	}
}
