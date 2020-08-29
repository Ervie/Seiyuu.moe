using SeiyuuMoe.AnimeComparisons;
using SeiyuuMoe.Application.AnimeComparisons.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.AnimeComparisons.CompareAnime
{
	public class CompareAnimeQueryHandler
	{
		private readonly IAnimeRoleRepository _roleRepository;

		public CompareAnimeQueryHandler(IAnimeRoleRepository animeRoleRepository)
		{
			_roleRepository = animeRoleRepository;
		}

		public async Task<ICollection<AnimeComparisonEntryDto>> HandleAsync(CompareAnimeQuery query)
		{
			ICollection<AnimeComparisonEntry> partialResults = new List<AnimeComparisonEntry>();

			for (int i = 0; i < query.AnimeMalIds.Count; i++)
			{
				var roles = await _roleRepository.GetAllRolesInAnimeByMalIdAsync(query.AnimeMalIds.ElementAt(i));

				foreach (var role in roles)
				{
					if (partialResults.Any(x => x.Seiyuu.Id.Equals(role.SeiyuuId)))
					{
						var foundSeiyuu = partialResults.Single(x => x.Seiyuu.Id.Equals(role.SeiyuuId));

						if (foundSeiyuu.AnimeCharacters.Any(x => x.Anime.Id.Equals(role.AnimeId)))
						{
							var foundAnime = foundSeiyuu.AnimeCharacters.Single(x => x.Anime.Id.Equals(role.AnimeId));
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

			var mappedEntities = partialResults.Select(x => x.ToAnimeComparisonEntryDto()).ToList();

			return mappedEntities;
		}
	}
}