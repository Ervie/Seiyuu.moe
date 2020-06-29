using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Context;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class AnimeStatusRepository : IAnimeStatusRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public AnimeStatusRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public Task<AnimeStatus> GetByNameAsync(string typeName)
			=> _dbContext.AnimeStatus.FirstOrDefaultAsync(x => x.Name == typeName);
	}
}