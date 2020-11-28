using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface ICharacterRepository
	{
		Task<AnimeCharacter> GetAsync(long characterMalId);

		Task AddAsync(AnimeCharacter character);

		Task UpdateAsync(AnimeCharacter character);

		Task<PagedResult<AnimeCharacter>> GetPageAsync(int page = 0, int pageSize = 100);

		Task<IReadOnlyList<AnimeCharacter>> GetOlderThanModifiedDate(DateTime olderThan, int pageSize = 150);

		Task<int> GetCountAsync();
	}
}