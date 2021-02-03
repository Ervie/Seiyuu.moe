using Microsoft.EntityFrameworkCore;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Warehouse.Repositories
{
	public class VndbCharacterVisualNovelRepository : IVndbCharacterVisualNovelRepository
	{
		private readonly WarehouseDbContext _warehouseDbContext;

		public VndbCharacterVisualNovelRepository(WarehouseDbContext warehouseDbContext)
		{
			_warehouseDbContext = warehouseDbContext;
		}

		public Task<VndbCharacterVisualNovel> GetAsync(int visualNovelVndbId, int characterVndbId)
			=> _warehouseDbContext.CharacterVisualNovels
			.SingleOrDefaultAsync(x => x.VisualNovelId == visualNovelVndbId && x.CharacterId == characterVndbId && x.IsSpoiler == 0);
	}
}