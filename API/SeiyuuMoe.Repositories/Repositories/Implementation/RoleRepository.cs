using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;
using System;
using System.Linq;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class RoleRepository : CRUDEntityFrameworkRepository<Role, ISeiyuuMoeContext, long>, IRoleRepository
	{
		public RoleRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}

		public override Func<IQueryable<Role>, IQueryable<Role>> IncludeExpression => anime => anime
			.Include(a => a.Anime)
			.Include(a => a.Character)
			.Include(a => a.Seiyuu);
	}
}