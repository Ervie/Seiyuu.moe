using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Animes
{
	public class AnimeTypeRepository : IAnimeTypeRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeTypeRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<AnimeType> GetByNameAsync(string typeName)
			=> _dbContext.AnimeTypes.FirstOrDefaultAsync(x => x.Description == typeName);
	}
}