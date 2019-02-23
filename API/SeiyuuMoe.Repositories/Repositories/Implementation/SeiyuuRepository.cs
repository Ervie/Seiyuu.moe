using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class SeiyuuRepository : CRUDEntityFrameworkRepository<Seiyuu, SeiyuuMoeContext, long>, ISeiyuuRepository
	{
		public SeiyuuRepository(SeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}
