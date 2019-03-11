using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class RoleRepository : CRUDEntityFrameworkRepository<Role, ISeiyuuMoeContext, long>, IRoleRepository
	{
		public RoleRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}