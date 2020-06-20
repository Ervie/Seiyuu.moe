using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Repositories.Repositories
{
	public interface IBlacklistedIdRepository
	{
		Task AddAsync(BlacklistedId blacklistedId);
	}
}