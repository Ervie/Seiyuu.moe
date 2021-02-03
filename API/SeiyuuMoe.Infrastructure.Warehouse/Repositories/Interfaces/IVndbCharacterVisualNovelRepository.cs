using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces
{
	public interface IVndbCharacterVisualNovelRepository
	{
		Task<VndbCharacterVisualNovel> GetAsync(int visualNovelVndbId, int characterVndbId);
	}
}