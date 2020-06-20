using SeiyuuMoe.BusinessServices.Extensions;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.WebEssentials;
using SeiyuuMoe.Repositories.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	internal class AnimeBusinessService : IAnimeBusinessService
	{
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeSearchCriteriaService _animeSearchCriteriaService;
		private readonly IAnimeRoleRepository _roleRepository;

		public AnimeBusinessService(
			IAnimeRepository animeRepository,
			IAnimeSearchCriteriaService animeSearchCriteriaService,
			IAnimeRoleRepository roleRepository)
		{
			_animeRepository = animeRepository;
			_animeSearchCriteriaService = animeSearchCriteriaService;
			_roleRepository = roleRepository;
		}

		public async Task<PagedResult<AnimeSearchEntryDto>> GetAsync(Query<AnimeSearchCriteria> query)
		{
			var expression = _animeSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await _animeRepository.GetOrderedPageAsync(expression, query.Page, query.PageSize);

			return entities.Map<Anime, AnimeSearchEntryDto>(entities.Results.Select(x => x.ToAnimeSearchEntryDto()));
		}

		public async Task<AnimeCardDto> GetSingleAsync(long id)
		{
			var entity = await _animeRepository.GetAsync(id);

			return entity.ToAnimeCardDto();
		}

		public async Task<ICollection<AnimeComparisonEntryDto>> GetAnimeComparison(AnimeComparisonSearchCriteria searchCriteria)
		{
			ICollection<AnimeComparisonEntry> partialResults = new List<AnimeComparisonEntry>();

			for (int i = 0; i < searchCriteria.AnimeMalId.Count; i++)
			{
				var roles = await _roleRepository.GetAllRolesInAnimeAsync(searchCriteria.AnimeMalId.ElementAt(i));

				foreach (var role in roles)
				{
					if (partialResults.Any(x => x.Seiyuu.MalId.Equals(role.SeiyuuId)))
					{
						var foundSeiyuu = partialResults.Single(x => x.Seiyuu.MalId.Equals(role.SeiyuuId));

						if (foundSeiyuu.AnimeCharacters.Any(x => x.Anime.MalId.Equals(role.AnimeId)))
						{
							var foundAnime = foundSeiyuu.AnimeCharacters.Single(x => x.Anime.MalId.Equals(role.AnimeId));
							foundAnime.Characters.Add(role.Character);
						}
						else
						{
							foundSeiyuu.AnimeCharacters.Add(new AnimeComparisonSubEntry(role.Character, role.Anime));
						}
					}
					else
					{
						AnimeComparisonEntry newComparisonEntry = new AnimeComparisonEntry
						{
							Seiyuu = role.Seiyuu
						};
						newComparisonEntry.AnimeCharacters.Add(new AnimeComparisonSubEntry(role.Character, role.Anime));
						partialResults.Add(newComparisonEntry);
					}
				}

				partialResults = partialResults.Where(x => x.AnimeCharacters.Select(y => y.Anime).ToList().Count >= i + 1).ToList();
			}

			return partialResults.Select(x => x.ToAnimeComparisonEntryDto()).ToList();
		}
	}
}