using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Application.Handlers
{
	public class InsertNewRolesHandler : IVndbJobHandler
	{
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly IVndbStaffAliasRepository _vndbStaffAliasRepository;
		private readonly IVisualNovelRepository _visualNovelRepository;
		private readonly IVisualNovelCharacterRepository _visualNovelCharacterRepository;
		private readonly IVisualNovelRoleRepository _visualNovelRoleRepository;
		private readonly IVndbCharacterVisualNovelRepository _vndbCharacterVisualNovelRepository;

		public InsertNewRolesHandler(
			ISeiyuuRepository seiyuuReposiory,
			IVndbStaffAliasRepository vndbStaffAliasRepository,
			IVisualNovelRepository visualNovelRepository,
			IVisualNovelCharacterRepository visualNovelCharacterRepository,
			IVisualNovelRoleRepository visualNovelRoleRepository,
			IVndbCharacterVisualNovelRepository vndbCharacterVisualNovelRepository
		)
		{
			_seiyuuRepository = seiyuuReposiory;
			_vndbStaffAliasRepository = vndbStaffAliasRepository;
			_visualNovelRepository = visualNovelRepository;
			_visualNovelCharacterRepository = visualNovelCharacterRepository;
			_visualNovelRoleRepository = visualNovelRoleRepository;
			_vndbCharacterVisualNovelRepository = vndbCharacterVisualNovelRepository;
		}

		public async Task HandleAsync()
		{
			Console.WriteLine("Started InsertNewRolesHandler job");

			var allVndbIds = await _seiyuuRepository.GetAllVndbIdsAsync();

			foreach (var vndbId in allVndbIds)
			{
				await InsertRolesAsync(vndbId);
			}

			Console.WriteLine("Finished InsertNewRolesHandler job");
		}

		private async Task InsertRolesAsync(long vndbId)
		{
			var seiyuu = await _seiyuuRepository.GetByVndbIdAsync(vndbId);

			var allRoles = await _vndbStaffAliasRepository.GetAllRoles(vndbId);

			foreach (var role in allRoles)
			{
				await InsertRoleAsync(role, seiyuu);
			}
		}

		private async Task InsertRoleAsync(VndbVisualNovelSeiyuu role, Seiyuu seiyuu)
		{
			var vn = await _visualNovelRepository.GetAsync(role.VisualNovelId);
			var character = await _visualNovelCharacterRepository.GetAsync(role.CharacterId);
			var appearance = await _vndbCharacterVisualNovelRepository.GetAsync(role.VisualNovelId, role.CharacterId);

			if (vn is null || character is null || appearance is null)
			{
				return;
			}

			var roleExists = await _visualNovelRoleRepository.RoleExistsAsync(vn.Id, character.Id, seiyuu.Id);

			if (!roleExists)
			{
				await _visualNovelRoleRepository.InsertRoleAsync(new VisualNovelRole
				{
					Id = Guid.NewGuid(),
					CharacterId = character.Id,
					LanguageId = LanguageId.Japanese,
					SeiyuuId = seiyuu.Id,
					VisualNovelId = vn.Id,
					RoleTypeId = Enum.Parse<VisualNovelRoleTypeId>(appearance.RoleType, true)
				});
			}
		}
	}
}