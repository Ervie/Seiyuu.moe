using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Context;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Animes
{
	public class AnimeStatusRepository : IAnimeStatusRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeStatusRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<AnimeStatus> GetByNameAsync(string typeName)
			=> _dbContext.AnimeStatuses.FirstOrDefaultAsync(x => x.Description == typeName);
	}
}