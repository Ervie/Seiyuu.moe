using SeiyuuMoe.Data.Context;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Generic;

namespace SeiyuuMoe.Repositories.Repositories
{
	public class CharacterRepository: CRUDEntityFrameworkRepository<Character, ISeiyuuMoeContext, long>, ICharacterRepository
	{
		public CharacterRepository(ISeiyuuMoeContext dbContext) : base(dbContext, x => x.Id)
		{
		}
	}
}
