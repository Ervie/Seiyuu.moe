using AutoMapper;
using SeiyuuMoe.BusinessServices.SearchCriteria;
using SeiyuuMoe.Common.Extensions;
using SeiyuuMoe.Contracts.ComparisonEntities;
using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Data.Model;
using SeiyuuMoe.Repositories.Models;
using SeiyuuMoe.Repositories.Repositories;
using SeiyuuMoe.WebEssentials;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.BusinessServices
{
	internal class AnimeBusinessService : IAnimeBusinessService
	{
		private readonly IMapper mapper;
		private readonly IAnimeRepository animeRepository;
		private readonly IAnimeSearchCriteriaService animeSearchCriteriaService;
		private readonly IRoleRepository roleRepository;

		public AnimeBusinessService(
			IMapper mapper,
			IAnimeRepository animeRepository,
			IAnimeSearchCriteriaService animeSearchCriteriaService,
			IRoleRepository roleRepository)
		{
			this.mapper = mapper;
			this.animeRepository = animeRepository;
			this.animeSearchCriteriaService = animeSearchCriteriaService;
			this.roleRepository = roleRepository;
		}

		public async Task<PagedResult<AnimeSearchEntryDto>> GetAsync(Query<AnimeSearchCriteria> query)
		{
			var expression = await animeSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await animeRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<AnimeSearchEntryDto>>(entities);
		}

		public async Task<PagedResult<AnimeAiringDto>> GetDatesAsync(Query<AnimeSearchCriteria> query)
		{
			var expression = await animeSearchCriteriaService.BuildExpression(query.SearchCriteria);

			var entities = await animeRepository.GetOrderedPageAsync(expression, query.SortExpression, query.Page, query.PageSize);

			return mapper.Map<PagedResult<AnimeAiringDto>>(entities);
		}

		public async Task<AnimeCardDto> GetSingleAsync(long id)
		{
			var entity = await animeRepository.GetAsync(x => x.MalId.Equals(id), animeRepository.IncludeExpression);

			return mapper.Map<AnimeCardDto>(entity);
		}

		public async Task<ICollection<AnimeComparisonEntryDto>> GetAnimeComparison(RoleSearchCriteria searchCriteria)
		{
			ICollection<AnimeComparisonEntry> partialResults = new List<AnimeComparisonEntry>();

			for (int i = 0; i < searchCriteria.AnimeMalId.Count; i++)
			{
				var roles = await roleRepository.GetAllAsync(x => x.AnimeId.Equals(searchCriteria.AnimeMalId.ToArray()[i]), roleRepository.IncludeExpression);

				foreach (var role in roles)
				{
					if (partialResults.Any(x => x.Seiyuu.MalId.Equals(role.SeiyuuId)))
					{
						partialResults.First(x => x.Seiyuu.MalId.Equals(role.SeiyuuId)).CharacterAnimePairs.Add(new CharacterAnimePair(role.Character, role.Anime));
					}
					else
					{
						AnimeComparisonEntry newComparisonEntry = new AnimeComparisonEntry
						{
							Seiyuu = role.Seiyuu
						};
						newComparisonEntry.CharacterAnimePairs.Add(new CharacterAnimePair(role.Character, role.Anime));
						partialResults.Add(newComparisonEntry);
					}
				}

				partialResults = partialResults.Where(x => x.CharacterAnimePairs.DistinctBy(y => y.Anime.MalId).ToList().Count >= i + 1).ToList();
			}

			return mapper.Map<ICollection<AnimeComparisonEntryDto>>(partialResults);
		}
	}
}