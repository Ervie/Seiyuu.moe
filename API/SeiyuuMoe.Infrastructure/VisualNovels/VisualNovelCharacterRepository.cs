using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.VisualNovels
{
	public class VisualNovelCharacterRepository : IVisualNovelCharacterRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public VisualNovelCharacterRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(VisualNovelCharacter visualNovelCharacter)
		{
			await _dbContext.AddAsync(visualNovelCharacter);
			await _dbContext.SaveChangesAsync();
		}

		public Task<VisualNovelCharacter> GetAsync(long vndbId)
			=> _dbContext.VisualNovelCharacters
			.FirstOrDefaultAsync(x => x.VndbId == vndbId);

		public async Task UpdateAsync(VisualNovelCharacter visualNovelCharacter)
		{
			visualNovelCharacter.ModificationDate = DateTime.UtcNow;
			_dbContext.VisualNovelCharacters.Update(visualNovelCharacter);
			await _dbContext.SaveChangesAsync();
		}
	}
}