using AutoMapper;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.Dtos.Other;
using SeiyuuMoe.Contracts.SearchCriteria;
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
			var expression = await seiyuuSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await seiyuuRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<SeiyuuSearchEntryDto>>(entities);
		}

		public async Task<SeiyuuCardDto> GetSingleAsync(long id)
		{
			var entity = await seiyuuRepository.GetAsync(x => x.MalId.Equals(id));

			return mapper.Map<SeiyuuCardDto>(entity);
		}

		public async Task<ICollection<SeiyuuComparisonEntryDto>> GetSeiyuuComparison(RoleSearchCriteria searchCriteria)
		{
			ICollection<SeiyuuComparisonEntry> partialResults = new List<SeiyuuComparisonEntry>();

			for (int i = 0; i < searchCriteria.SeiyuuMalId.Count; i++)
			{
				var roles = await roleRepository.GetAllAsync(x =>
					x.SeiyuuId.Equals(searchCriteria.SeiyuuMalId.ToArray()[i]) &&
					x.LanguageId == 1,
					roleRepository.IncludeExpression);

				foreach (var role in roles)
				{
					if (partialResults.Any(x => x.Anime.MalId.Equals(role.AnimeId)))
					{
						var foundAnime = partialResults.Single(x => x.Anime.MalId.Equals(role.AnimeId));

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
						SeiyuuComparisonEntry newComparisonEntry = new SeiyuuComparisonEntry
						{
							Anime = role.Anime
						};
						newComparisonEntry.SeiyuuCharacters.Add(new SeiyuuComparisonSubEntry(role.Character, role.Seiyuu));
						partialResults.Add(newComparisonEntry);
					}
				}

				partialResults = partialResults.Where(x => x.SeiyuuCharacters.Select(y => y.Seiyuu).ToList().Count >= i + 1).ToList();
			}

			return mapper.Map<ICollection<SeiyuuComparisonEntryDto>>(partialResults);
		}
	}
}