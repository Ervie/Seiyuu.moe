using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Infrastructure.Context;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class BlacklistedIdRepository : IBlacklistedIdRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public BlacklistedIdRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(BlacklistedId blacklistedId)
		{
			await _dbContext.BlacklistedId.AddAsync(blacklistedId);
			await _dbContext.SaveChangesAsync();
		}
	}
}