using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;
using System;
using System.Linq;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class AnimeRepository : CRUDEntityFrameworkRepository<Anime, ISeiyuuMoeContext, long>, IAnimeRepository
	{
		public AnimeRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}

		public override Func<IQueryable<Anime>, IQueryable<Anime>> IncludeExpression => anime => anime
			.Include(a => a.Type)
			.Include(a => a.Status)
			.Include(a => a.Season);
	}
}
