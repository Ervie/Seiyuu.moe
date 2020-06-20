using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface ICharacterRepository
	{
		Task<Character> GetAsync(long characterMalId);

		Task AddAsync(Character character);

		Task UpdateAsync(Character character);

		Task<PagedResult<Character>> GetPageAsync(int page, int pageSize);

		Task<int> GetCharactersCountAsync();
	}
}