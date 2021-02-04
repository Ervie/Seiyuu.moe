using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;
using SeiyuuMoe.Infrastructure.Database.VisualNovels;
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
			//CreateAddOrUpdateCharactersHandler(),k
			//CreateMatchSeiyuuHandler()
			CreateInsertNewRolesHandler()
		};

		private IVndbJobHandler CreateInsertNewRolesHandler()
			=> new InsertNewRolesHandler(
				new SeiyuuRepository(_seiyuuMoeContext),
				new VndbStaffAliasRepository(_warehouseDbContext),
				new VisualNovelRepository(_seiyuuMoeContext),
				new VisualNovelCharacterRepository(_seiyuuMoeContext),
				new VisualNovelRoleRepository(_seiyuuMoeContext),
				new VndbCharacterVisualNovelRepository(_warehouseDbContext)
			);

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