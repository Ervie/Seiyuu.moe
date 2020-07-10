using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ICharacterRepository
	{
		Task<Character> GetAsync(long characterMalId);

		Task AddAsync(Character character);

		Task UpdateAsync(Character character);

		Task<PagedResult<Character>> GetPageAsync(int page = 0, int pageSize = 100);

		Task<int> GetCountAsync();
	}
}