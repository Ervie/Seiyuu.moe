using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IBlacklistedIdRepository
	{
		Task AddAsync(Blacklist blacklistedId);
	}
}