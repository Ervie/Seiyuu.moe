using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;
using System;
using System.Linq;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class CharacterRepository: CRUDEntityFrameworkRepository<Character, ISeiyuuMoeContext, long>, ICharacterRepository
	{
		public CharacterRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.MalId)
		{
		}

		public override Func<IQueryable<Character>, IQueryable<Character>> IncludeExpression => anime => anime
			.Include(a => a.Role);
	}
}
