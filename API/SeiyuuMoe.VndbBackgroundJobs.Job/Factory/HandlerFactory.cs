using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.Infrastructure.VisualNovels;
using SeiyuuMoe.Infrastructure.Warehouse;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories;
using SeiyuuMoe.VndbBackgroundJobs.Application.Handlers;
using System.Collections.Generic;

namespace SeiyuuMoe.VndbBackgroundJobs.Job.Factory
{
	public class HandlerFactory
	{
		private readonly SeiyuuMoeContext _seiyuuMoeContext;
		private readonly WarehouseDbContext _warehouseDbContext;

		public HandlerFactory(SeiyuuMoeContext seiyuuMoeContext, WarehouseDbContext warehouseDbContext)
		{
			_seiyuuMoeContext = seiyuuMoeContext;
			_warehouseDbContext = warehouseDbContext;
		}

		public ICollection<IVndbJobHandler> CreateAllHandlers() => new List<IVndbJobHandler>
		{
			//CreateAddOrUpdateVisualNovelsHandler(),
			//CreateAddOrUpdateCharactersHandler(),
			CreateMatchSeiyuuHandler()
		};

		private IVndbJobHandler CreateMatchSeiyuuHandler() =>
			new MatchSeiyuuHandler(
				new VndbStaffAliasRepository(_warehouseDbContext),
				new VndbStaffRepository(_warehouseDbContext),
				new SeiyuuRepository(_seiyuuMoeContext)
			);

		private IVndbJobHandler CreateAddOrUpdateCharactersHandler() =>
			new AddOrUpdateVisualNovelCharactersHandler(
				new VisualNovelCharacterRepository(_seiyuuMoeContext),
				new VndbCharacterRepository(_warehouseDbContext)
			);

		private IVndbJobHandler CreateAddOrUpdateVisualNovelsHandler() =>
			new AddOrUpdateVisualNovelsHandler(
				new VisualNovelRepository(_seiyuuMoeContext),
				new VndbVisualNovelRepository(_warehouseDbContext)
			);
	}
}