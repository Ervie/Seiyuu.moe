using SeiyuuMoe.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Repositories.Generic
{
	public class EntityFrameworkRepository<TContext>
	   where TContext : ISeiyuuMoeContext
	{
		protected TContext Context { get; }

		protected EntityFrameworkRepository(TContext dbContext)
		{
			Context = dbContext;
		}
	}
}
