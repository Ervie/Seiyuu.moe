using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	internal class SeasonBusinessService : ISeasonBusinessService
	{
		private readonly ISeasonRepository seasonRepository;
		private readonly IAnimeRepository animeRepository;
		private readonly IRoleRepository roleRepository;
		private readonly IRoleSearchCriteriaService roleSearchCriteriaService;
		private readonly IAnimeSearchCriteriaService animeSearchCriteriaService;
		private readonly ISeasonSearchCriteriaService seasonSearchCriteriaService;

		public SeasonBusinessService(
			ISeasonRepository seasonRepository,
			IAnimeRepository animeRepository,
			IRoleRepository roleRepository,
			IRoleSearchCriteriaService roleSearchCriteriaService,
			IAnimeSearchCriteriaService animeSearchCriteriaService,
			ISeasonSearchCriteriaService seasonSearchCriteriaService
		)
		{
			this.seasonRepository = seasonRepository;
			this.animeRepository = animeRepository;
			this.roleRepository = roleRepository;
			this.roleSearchCriteriaService = roleSearchCriteriaService;
			this.animeSearchCriteriaService = animeSearchCriteriaService;
			this.seasonSearchCriteriaService = seasonSearchCriteriaService;
		}

		public async Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonRolesSummary(Query<SeasonSummarySearchCriteria> query)
		{
			var seasonPredicate = seasonSearchCriteriaService.BuildExpression(query.SearchCriteria.ToSeasonSearchCriteria());

			var foundSeason = await seasonRepository.GetAsync(seasonPredicate);

			if (foundSeason != null)
			{
				query.SearchCriteria.Id = foundSeason.Id;
				var animePredicate = animeSearchCriteriaService.BuildExpression(query.SearchCriteria.ToAnimeSearchCriteria());

				var animeInSeason = await animeRepository.GetAllAsync(animePredicate);

				if (animeInSeason.Count > 0)
				{
					query.SearchCriteria.AnimeId = animeInSeason.Select(x => x.MalId).ToList();

					var rolePredicate = roleSearchCriteriaService.BuildExpression(query.SearchCriteria.ToRoleSearchCriteria());

					IReadOnlyCollection<Role> allRolesInSeason = await roleRepository.GetAllAsync(rolePredicate, roleRepository.IncludeExpression);

					var groupedRoles = GroupRoles(allRolesInSeason, query);

					return groupedRoles.Map<SeasonSummaryEntry, SeasonSummaryEntryDto>(groupedRoles.Results.Select(x => x.ToSeasonSummaryEntryDto()));
				}
			}

			return null;
		}

		public PagedResult<SeasonSummaryEntry> GroupRoles(IReadOnlyCollection<Role> roles, Query<SeasonSummarySearchCriteria> query)
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