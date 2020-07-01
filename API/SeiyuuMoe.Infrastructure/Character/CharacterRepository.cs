using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Context;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class CharacterRepository : ICharacterRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public CharacterRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Character character)
		{
			await _dbContext.AddAsync(character);
			await _dbContext.SaveChangesAsync();
		}

		public Task<Character> GetAsync(long characterMalId)
			=> _dbContext.Character.FirstOrDefaultAsync(x => x.MalId == characterMalId);

		public Task<int> GetCountAsync() => _dbContext.Character.CountAsync();

		public async Task<PagedResult<Character>> GetPageAsync(int page = 0, int pageSize = 100)
		{
			var totalCount = await _dbContext.Character.CountAsync();
			var results = await _dbContext.Character
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<Character>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task UpdateAsync(Character character)
		{
			_dbContext.Update(character);
			await _dbContext.SaveChangesAsync();
		}
	}
}