using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.Season.GetSeasonSummaries
{
	public class GetSeasonSummariesQueryHandler
	{
		private readonly ISeasonRepository _seasonRepository;
		private readonly IAnimeRepository _animeRepository;
		private readonly ISeasonRoleRepository _seasonRoleRepository;
		private readonly IAnimeSearchCriteriaService _animeSearchCriteriaService;
		private readonly ISeasonSearchCriteriaService _seasonSearchCriteriaService;

		public GetSeasonSummariesQueryHandler(
			ISeasonRepository seasonRepository,
			IAnimeRepository animeRepository,
			ISeasonRoleRepository roleRepository,
			IAnimeSearchCriteriaService animeSearchCriteriaService,
			ISeasonSearchCriteriaService seasonSearchCriteriaService
			)
		{
			_seasonRepository = seasonRepository;
			_animeRepository = animeRepository;
			_seasonRoleRepository = roleRepository;
			_animeSearchCriteriaService = animeSearchCriteriaService;
			_seasonSearchCriteriaService = seasonSearchCriteriaService;
		}

		public async Task<QueryResponse<PagedResult<SeasonSummaryEntryDto>>> HandleAsync(GetSeasonSummariesQuery query)
		{
			var seasonPredicate = _seasonSearchCriteriaService.BuildExpression(query);

			var foundSeason = await _seasonRepository.GetAsync(seasonPredicate);

			if (foundSeason != null)
			{
				query.Id = foundSeason.Id;
				var animePredicate = _animeSearchCriteriaService.BuildExpression(query.ToSearchAnimeQuery());

				var animeInSeason = await _animeRepository.GetAllAsync(animePredicate);

				if (animeInSeason.Any())
				{
					var animeInSeasonIds = animeInSeason.Select(x => x.MalId).ToList();

					IReadOnlyCollection<Role> allRolesInSeason = await _seasonRoleRepository.GetAllRolesInSeason(
						animeInSeasonIds,
						query.MainRolesOnly
					);

					var groupedRoles = GroupRoles(allRolesInSeason, query);

					var mappedRoles = groupedRoles.Map<SeasonSummaryEntry, SeasonSummaryEntryDto>(groupedRoles.Results.Select(x => x.ToSeasonSummaryEntryDto()));

					return new QueryResponse<PagedResult<SeasonSummaryEntryDto>>(mappedRoles);
				}
			}

			return null;
		}

		private PagedResult<SeasonSummaryEntry> GroupRoles(IReadOnlyCollection<Role> roles, GetSeasonSummariesQuery query)
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
					.ThenByDescending(x => x.GetTotalSignificanceValue())
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