using AutoMapper;
using SeiyuuMoe.BusinessServices;
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
using System.Text;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	class SeasonBusinessService: ISeasonBusinessService
	{
		private readonly IMapper mapper;
		private readonly ISeasonRepository seasonRepository;
		private readonly IAnimeRepository animeRepository;
		private readonly IRoleRepository roleRepository;
		private readonly IRoleSearchCriteriaService roleSearchCriteriaService;


		public SeasonBusinessService(
			IMapper mapper,
			ISeasonRepository seasonRepository,
			IAnimeRepository animeRepository,
			IRoleRepository roleRepository,
			IRoleSearchCriteriaService roleSearchCriteriaService
		)
		{
			this.mapper = mapper;
			this.seasonRepository = seasonRepository;
			this.animeRepository = animeRepository;
			this.roleRepository = roleRepository;
			this.roleSearchCriteriaService = roleSearchCriteriaService;
		}

		public async Task<PagedResult<SeasonSummaryEntryDto>> GetSeasonRolesSummary(Query<SeasonSearchCriteria> query)
		{
			var foundSeason = await seasonRepository.GetAsync(x => 
				x.Name.Equals(query.SearchCriteria.Season, StringComparison.CurrentCultureIgnoreCase) && 
				x.Year.Equals(query.SearchCriteria.Year));

			if (foundSeason != null)
			{
				var animeInSeason = await animeRepository.GetAllAsync(x => x.SeasonId.Equals(foundSeason.Id) && x.TypeId == 1); //TV Only for now

				RoleSearchCriteria roleSearchCriteria = new RoleSearchCriteria()
				{
					AnimeId = animeInSeason.Select(x => x.MalId).ToList()
				};

				var rolePredicate = roleSearchCriteriaService.BuildExpression(roleSearchCriteria);

				IReadOnlyCollection<Role> allRolesInSeason = await roleRepository.GetAllAsync(rolePredicate, roleRepository.IncludeExpression);

				return mapper.Map<PagedResult<SeasonSummaryEntryDto>>(GroupRoles(allRolesInSeason, query));
			}
			else
			{
				return null;
			}
		}

		public PagedResult<SeasonSummaryEntry> GroupRoles(IReadOnlyCollection<Role> roles, Query<SeasonSearchCriteria> query)
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
						.AnimeCharacterPairs.Add(new Tuple<Anime, Character>(role.Anime, role.Character));
				}
			}

			return new PagedResult<SeasonSummaryEntry>()
			{
				Results = groupedEntities
					.OrderByDescending(x => x.AnimeCharacterPairs.Count)
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
