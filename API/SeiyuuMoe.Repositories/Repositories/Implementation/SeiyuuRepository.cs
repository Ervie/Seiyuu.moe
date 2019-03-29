using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;
using System;
using System.Linq;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class SeiyuuRepository : CRUDEntityFrameworkRepository<Seiyuu, ISeiyuuMoeContext, long>, ISeiyuuRepository
	{
		public SeiyuuRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.MalId)
		{
		}

		public override Func<IQueryable<Seiyuu>, IQueryable<Seiyuu>> IncludeExpression => anime => anime
			.Include(a => a.Role);
	}
}
