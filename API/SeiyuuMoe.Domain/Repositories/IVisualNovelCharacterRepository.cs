using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IVisualNovelCharacterRepository
	{
		Task<VisualNovelCharacter> GetAsync(long vndbId);

		Task AddAsync(VisualNovelCharacter visualNovelCharacter);

		Task UpdateAsync(VisualNovelCharacter visualNovelCharacter);
	}
}