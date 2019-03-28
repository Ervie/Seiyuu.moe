
using System.Threading.Tasks;

namespace SeiyuuMoe.JikanToDBParser
{
	public interface IJikanParser
	{
		Task InsertSeiyuu();

		Task ParseRoles();

		Task UpdateCharacters();

		Task UpdateAnime();

		Task UpdateSeasons();

		Task UpdateSeiyuu();
	}
}
