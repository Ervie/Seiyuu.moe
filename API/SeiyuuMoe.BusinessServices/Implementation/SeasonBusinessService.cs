using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	internal class SeasonBusinessService : ISeasonBusinessService
	{
		private readonly ISeasonRepository _seasonRepository;
		private readonly IAnimeRepository _animeRepository;
		private readonly ISeasonRoleRepository _roleRepository;
		private readonly IRoleSearchCriteriaService _roleSearchCriteriaService;
		private readonly IAnimeSearchCriteriaService _animeSearchCriteriaService;
		private readonly ISeasonSearchCriteriaService _seasonSearchCriteriaService;

		public SeasonBusinessService(
			ISeasonRepository seasonRepository,
			IAnimeRepository animeRepository,
			ISeasonRoleRepository roleRepository,
			IRoleSearchCriteriaService roleSearchCriteriaService,
			IAnimeSearchCriteriaService animeSearchCriteriaService,
			ISeasonSearchCriteriaService seasonSearchCriteriaService
		)
		{
			_seasonRepository = seasonRepository;
			_animeRepository = animeRepository;
			_roleRepository = roleRepository;
			_roleSearchCriteriaService = roleSearchCriteriaService;
			_animeSearchCriteriaService = animeSearchCriteriaService;
			_seasonSearchCriteriaService = seasonSearchCriteriaService;
		}

		public async Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonRolesSummary(Query<SeasonSummarySearchCriteria> query)
		{
			var seasonPredicate = _seasonSearchCriteriaService.BuildExpression(query.SearchCriteria.ToSeasonSearchCriteria());

			var foundSeason = await _seasonRepository.GetAsync(seasonPredicate);

			if (foundSeason != null)
			{
				query.SearchCriteria.Id = foundSeason.Id;
				var animePredicate = _animeSearchCriteriaService.BuildExpression(query.SearchCriteria.ToAnimeSearchCriteria());

				var animeInSeason = await _animeRepository.GetAllAsync(animePredicate);

				if (animeInSeason.Any())
				{
					var animeInSeasonIds = animeInSeason.Select(x => x.MalId).ToList();

					IReadOnlyCollection<Role> allRolesInSeason = await _roleRepository.GetAllRolesInSeason(
						animeInSeasonIds,
						query.SearchCriteria.MainRolesOnly
					);

					var groupedRoles = GroupRoles(allRolesInSeason, query);

					return groupedRoles.Map<SeasonSummaryEntry, SeasonSummaryEntryDto>(groupedRoles.Results.Select(x => x.ToSeasonSummaryEntryDto()));
				}
			}

			return null;
		}

		private PagedResult<SeasonSummaryEntry> GroupRoles(IReadOnlyCollection<Role> roles, Query<SeasonSummarySearchCriteria> query)
		{
			ICollection<SeasonSummaryEntry> groupedEntities = new List<SeasonSummaryEntry>();

			foreach (Role role in roles)
			{
				if (!groupedEntities.Select(x => x.Seiyuu.MalId).Contains(role.SeiyuuId.Value))
				{
					groupedEntities.Add(new SeasonSummaryEntry(role.Seiyuu, role.Anime, role.Character));
				}
				else
				{
					groupedEntities
						.Single(x => x.Seiyuu.MalId.Equals(role.SeiyuuId.Value))
						.AnimeCharacterPairs.Add((role.Anime, role.Character));
				}
			}

			return new PagedResult<SeasonSummaryEntry>()
			{
				Results = groupedEntities
					.OrderByDescending(x => x.AnimeCharacterPairs.Count)
					.ThenByDescending(x => x.TotalSignificanceValue)
					.Skip(query.Page * query.PageSize)
					.Take(query.PageSize)
					.ToList(),
				Page = query.Page,
				PageSize = query.PageSize,
				TotalCount = groupedEntities.Count()
			};
		}
	}
}