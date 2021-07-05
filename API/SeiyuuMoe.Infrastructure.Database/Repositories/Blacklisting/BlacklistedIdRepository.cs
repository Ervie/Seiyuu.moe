using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Context;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Database.Blacklisting
{
	public class BlacklistedIdRepository : IBlacklistedIdRepository
	{
		private readonly SeiyuuMoeContext _dbContext;

		public BlacklistedIdRepository(SeiyuuMoeContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddAsync(Blacklist blacklistedId)
		{
			await _dbContext.Blacklists.AddAsync(blacklistedId);
			await _dbContext.SaveChangesAsync();
		}
	}
}