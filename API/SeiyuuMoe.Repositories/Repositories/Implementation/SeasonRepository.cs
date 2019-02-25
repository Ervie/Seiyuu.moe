using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class SeasonRepository : CRUDEntityFrameworkRepository<Season, ISeiyuuMoeContext, long>, ISeasonRepository
	{
		public SeasonRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}
