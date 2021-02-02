using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Application.Handlers
{
	public class MatchSeiyuuHandler : IVndbJobHandler
	{
		private readonly IVndbStaffAliasRepository _vndbStaffAliasRepository;
		private readonly IVndbStaffRepository _vndbStaffRepository;
		private readonly ISeiyuuRepository _seiyuuRepository;

		public MatchSeiyuuHandler(
			IVndbStaffAliasRepository vndbStaffAliasRepository,
			IVndbStaffRepository vndbStaffRepository,
			ISeiyuuRepository seiyuuRepository
		)
		{
			_vndbStaffAliasRepository = vndbStaffAliasRepository;
			_vndbStaffRepository = vndbStaffRepository;
			_seiyuuRepository = seiyuuRepository;
		}

		public async Task HandleAsync()
		{
			Console.WriteLine("Started MatchSeiyuuHandler job");

			var distinctStaffAliases = await _vndbStaffAliasRepository.GetDistinctSeiyuuIdsAsync();

			foreach (var seiyuuId in distinctStaffAliases)
			{
				await UpdateSeiyuuReferenceAsync(seiyuuId);
			}

			Console.WriteLine("Finished MatchSeiyuuHandler job");
		}

		private async Task UpdateSeiyuuReferenceAsync(int seiyuuId)
		{
			var existingSeiyuu = await _seiyuuRepository.GetByVndbIdAsync(seiyuuId);

			if (existingSeiyuu != null)
			{
				return;
			}

			var vndbSeiyuu = await _vndbStaffRepository.GetSeiyuuAsync(seiyuuId);
			var matchedExistingSeiyuu = await _seiyuuRepository.GetByKanjiAsync(vndbSeiyuu.MainAlias.OriginalName);

			if (matchedExistingSeiyuu != null)
			{
				matchedExistingSeiyuu.VndbId = vndbSeiyuu.Id;
				await _seiyuuRepository.UpdateAsync(matchedExistingSeiyuu);
			}
		}
	}
}
