using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Repositories.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.SerBusinessServicesvices
{
	internal class SeiyuuBusinessService : ISeiyuuBusinessService
	{
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeiyuuSearchCriteriaService _seiyuuSearchCriteriaService;
		private readonly ISeiyuuRoleRepository _roleRepository;

		public SeiyuuBusinessService(
			ISeiyuuRepository seiyuuRepository,
			ISeiyuuSearchCriteriaService seiyuuSearchCriteriaService,
			ISeiyuuRoleRepository roleRepository)
		{
			_seiyuuRepository = seiyuuRepository;
			_seiyuuSearchCriteriaService = seiyuuSearchCriteriaService;
			_roleRepository = roleRepository;
		}

		public async Task<PagedResult<SeiyuuSearchEntryDto>> GetAsync(Query<SeiyuuSearchCriteria> query)
		{
			var expression = _seiyuuSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await _seiyuuRepository.GetOrderedPageAsync(expression, query.Page, query.PageSize);

			return entities.Map<Seiyuu, SeiyuuSearchEntryDto>(entities.Results.Select(x => x.ToSeiyuuSearchEntryDto()));
		}

		public async Task<SeiyuuCardDto> GetSingleAsync(long id)
		{
			var entity = await _seiyuuRepository.GetAsync(id);

			return entity.ToSeiyuuCardDto();
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

			partialResults = searchCriteria.GroupByFranchise ?
				GroupByFranchise(partialResults) :
				partialResults;

			return partialResults.Select(x => x.ToSeiyuuComparisonEntryDto()).ToList();
		}

		private async Task<IReadOnlyCollection<Role>> GetSeiyuuRoles(long seiyuuMalId, bool? mainRolesOnly)
			=> await _roleRepository.GetAllSeiyuuRolesAsync(seiyuuMalId, mainRolesOnly ?? false);

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