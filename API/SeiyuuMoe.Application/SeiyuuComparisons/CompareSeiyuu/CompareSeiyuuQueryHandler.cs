using SeiyuuMoe.Application.SeiyuuComparisons.Extensions;
using SeiyuuMoe.Domain.ComparisonEntities;
using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.WebEssentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu
{
	public class CompareSeiyuuQueryHandler
	{
		private readonly ISeiyuuRoleRepository _seiyuuRoleRepository;

		public CompareSeiyuuQueryHandler(ISeiyuuRoleRepository seiyuuRoleRepository)
		{
			_seiyuuRoleRepository = seiyuuRoleRepository;
		}

		public async Task<QueryResponse<ICollection<SeiyuuComparisonEntryDto>>> HandleAsync(CompareSeiyuuQuery query)
		{
			ICollection<SeiyuuComparisonEntry> partialResults = new List<SeiyuuComparisonEntry>();

			for (int i = 0; i < query.SeiyuuMalIds.Count; i++)
			{
				var roles = await GetSeiyuuRoles(query.SeiyuuMalIds.ToArray()[i], query.MainRolesOnly);

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

			partialResults = query.GroupByFranchise ?
				GroupByFranchise(partialResults) :
				partialResults;

			var mappedResults = partialResults.Select(x => x.ToSeiyuuComparisonEntryDto()).ToList();

			return new QueryResponse<ICollection<SeiyuuComparisonEntryDto>>(mappedResults);
		}

		private async Task<IReadOnlyCollection<AnimeRole>> GetSeiyuuRoles(long seiyuuMalId, bool? mainRolesOnly)
			=> await _seiyuuRoleRepository.GetAllSeiyuuRolesByMalIdAsync(seiyuuMalId, mainRolesOnly ?? false);

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
						foreach (var character in seiyuuCharacters.Characters)
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