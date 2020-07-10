using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IAnimeStatusRepository
	{
		Task<AnimeStatus> GetByNameAsync(string typeName);
	}
}