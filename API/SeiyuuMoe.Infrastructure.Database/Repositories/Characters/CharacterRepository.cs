using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.ScheduleItems;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Infrastructure.Database.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.Characters
{
	public class CharacterRepository : ICharacterRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public CharacterRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(AnimeCharacter character)
		{
			await _dbContext.AddAsync(character);
			await _dbContext.SaveChangesAsync();
		}

		public Task<AnimeCharacter> GetAsync(long characterMalId)
			=> _dbContext.AnimeCharacters.FirstOrDefaultAsync(x => x.MalId == characterMalId);

		public Task<int> GetCountAsync() => _dbContext.AnimeCharacters.CountAsync();

		public async Task<PagedResult<AnimeCharacter>> GetPageAsync(int page = 0, int pageSize = 100)
		{
			var totalCount = await _dbContext.AnimeCharacters.CountAsync();
			var results = await _dbContext.AnimeCharacters
				.Skip(page * pageSize)
				.Take(pageSize)
				.ToListAsync();

			return new PagedResult<AnimeCharacter>
			{
				Results = results,
				Page = page,
				PageSize = pageSize,
				TotalCount = totalCount
			};
		}

		public async Task<IReadOnlyList<CharacterScheduleItem>> GetOlderThanModifiedDate(DateTime olderThan, int pageSize = 150, DateTime? afterModificationDate = null, Guid? afterId = null)
		{
			var query = _dbContext.AnimeCharacters.Where(x => x.ModificationDate < olderThan);

			if (afterModificationDate.HasValue && afterId.HasValue)
			{
				query = query.Where(x =>
					x.ModificationDate > afterModificationDate.Value ||
					(x.ModificationDate == afterModificationDate.Value && x.Id.CompareTo(afterId.Value) > 0));
			}

			return await query
				.OrderBy(x => x.ModificationDate)
				.ThenBy(x => x.Id)
				.Take(pageSize)
				.Select(x => new CharacterScheduleItem(x.Id, x.MalId, x.ModificationDate))
				.ToListAsync();
		}

		public async Task UpdateAsync(AnimeCharacter character)
		{
			character.ModificationDate = DateTime.UtcNow;
			_dbContext.Update(character);
			await _dbContext.SaveChangesAsync();
		}
	}
}