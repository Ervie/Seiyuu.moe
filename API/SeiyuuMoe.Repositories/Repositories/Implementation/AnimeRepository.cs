using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class AnimeRepository : CRUDEntityFrameworkRepository<Anime, ISeiyuuMoeContext, long>, IAnimeRepository
	{
		public AnimeRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}
