using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class AnimeStatusRepository : CRUDEntityFrameworkRepository<AnimeStatus, ISeiyuuMoeContext, long>, IAnimeStatusRepository
	{
		public AnimeStatusRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}
