using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Context;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class AnimeTypeRepository : IAnimeTypeRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeTypeRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<AnimeType> GetByNameAsync(string typeName)
			=> _dbContext.AnimeType.FirstOrDefaultAsync(x => x.Name == typeName);
	}
}