using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IAnimeTypeRepository
	{
		Task<AnimeType> GetByNameAsync(string typeName);
	}
}