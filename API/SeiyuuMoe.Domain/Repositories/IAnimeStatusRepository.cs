using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface IAnimeStatusRepository
	{
		Task<AnimeStatus> GetByName(string typeName);
	}
}