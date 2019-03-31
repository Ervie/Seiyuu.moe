using AutoMapper;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.SerBusinessServicesvices
{
	internal class SeiyuuBusinessService : ISeiyuuBusinessService
	{
		private readonly IMapper mapper;
		private readonly ISeiyuuRepository seiyuuRepository;
		private readonly ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService;
		private readonly IRoleRepository roleRepository;

		public SeiyuuBusinessService(
			IMapper mapper,
			ISeiyuuRepository seiyuuRepository,
			ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService,
			IRoleRepository roleRepository)
		{
			this.mapper = mapper;
			this.seiyuuRepository = seiyuuRepository;
			this.seiyuuSearchCriteriaService = seiyuuSearchCriteriaService;
			this.roleRepository = roleRepository;
		}

		public async Task<PagedResult<SeiyuuSearchEntryDto>> GetAsync(Query<SeiyuuSearchCriteria> query)
		{
			var expression = seiyuuSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await seiyuuRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<SeiyuuSearchEntryDto>>(entities);
		}

		public async Task<SeiyuuCardDto> GetSingleAsync(long id)
		{
			var entity = await seiyuuRepository.GetAsync(x => x.MalId.Equals(id));

			return mapper.Map<SeiyuuCardDto>(entity);
		}

		public async Task<ICollection<SeiyuuComparisonEntryDto>> GetSeiyuuComparison(SeiyuuComparisonSearchCriteria searchCriteria)
		{
			ICollection<SeiyuuComparisonEntry> partialResults = new List<SeiyuuComparisonEntry>();

			for (int i = 0; i < searchCriteria.SeiyuuMalId.Count; i++)
			{
				var roles = await GetSeiyuuRoles(searchCriteria.SeiyuuMalId.ToArray()[i], searchCriteria.MainRolesOnly);

				foreach (var role in roles)
				{
					if (partialResults.Any(x => x.Anime.First().MalId.Equals(role.AnimeId)))
					{
						var foundAnime = partialResults.Single(x => x.Anime.First().MalId.Equals(role.AnimeId));

						if (foundAnime.SeiyuuCharacters.Any(x => x.Seiyuu.MalId.Equals(role.SeiyuuId)))
						{
							var foundSeiyuu = foundAnime.SeiyuuCharacters.Single(x => x.Seiyuu.MalId.Equals(role.SeiyuuId));
							foundSeiyuu.Characters.Add(role.Character);
						}
						else
						{
							foundAnime.SeiyuuCharacters.Add(new SeiyuuComparisonSubEntry(role.Character, role.Seiyuu));
						}
					}
					else
					{
						SeiyuuComparisonEntry newComparisonEntry = new SeiyuuComparisonEntry(role.Anime, role.Character, role.Seiyuu);
						partialResults.Add(newComparisonEntry);
					}
				}

				partialResults = partialResults.Where(x => x.SeiyuuCharacters.Select(y => y.Seiyuu).ToList().Count >= i + 1).ToList();
			}

			partialResults = searchCriteria.GroupByFranchise.HasValue && searchCriteria.GroupByFranchise.Value ?
				GroupByFranchise(partialResults) :
				partialResults;

			return mapper.Map<ICollection<SeiyuuComparisonEntryDto>>(partialResults);
		}

		private async Task<IReadOnlyCollection<Role>> GetSeiyuuRoles(long seiyuuMalId, bool? mainRolesOnly)
		{
			if (mainRolesOnly.HasValue && mainRolesOnly.Value)
			{
				return await roleRepository.GetAllAsync(x =>
					x.SeiyuuId.Equals(seiyuuMalId) &&
					x.LanguageId == 1 &&
					x.RoleTypeId == 1,
					roleRepository.IncludeExpression);
			}
			else
			{
				return await roleRepository.GetAllAsync(x =>
					x.SeiyuuId.Equals(seiyuuMalId) &&
					x.LanguageId == 1,
					roleRepository.IncludeExpression);
			}
		}

		private ICollection<SeiyuuComparisonEntry> GroupByFranchise(ICollection<SeiyuuComparisonEntry> nonGroupedResults)
		{
			ICollection<SeiyuuComparisonEntry> groupedResults = new List<SeiyuuComparisonEntry>();

			foreach (SeiyuuComparisonEntry entry in nonGroupedResults)
			{
				// Todo: change it to use relation between anime (when it's implemented).
				// Looks for characters in each comparison entry. Very confusing
				var franchiseFound = groupedResults
					.FirstOrDefault(x => x.SeiyuuCharacters.SelectMany(y => y.Characters.Select(z => z.MalId))
					.Intersect(entry.SeiyuuCharacters.SelectMany(q => q.Characters.Select(w => w.MalId)))
					.Any());

				if (franchiseFound != null)
				{
					franchiseFound.Anime.Add(entry.Anime.First());
					foreach (SeiyuuComparisonSubEntry seiyuuCharacters in entry.SeiyuuCharacters)
					{
						var foundSeiyuu = franchiseFound.SeiyuuCharacters.Single(x => x.Seiyuu.MalId.Equals(seiyuuCharacters.Seiyuu.MalId));
						foreach (Character character in seiyuuCharacters.Characters)
						{
							if (!foundSeiyuu.Characters.Select(x => x.MalId).Contains(character.MalId))
							{
								foundSeiyuu.Characters.Add(character);
							}
						}
					}
				}
				else
				{
					groupedResults.Add(entry);
				}
			}

			return groupedResults;
		}
	}
}