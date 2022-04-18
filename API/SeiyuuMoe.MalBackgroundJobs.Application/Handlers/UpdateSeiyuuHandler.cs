using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.MalBackgroundJobs.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class UpdateSeiyuuHandler
	{
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly IAnimeRepository _animeRepository;
		private readonly ICharacterRepository _characterRepository;
		private readonly ISeiyuuRoleRepository _seiyuuRoleRepository;
		private readonly IAnimeRoleRepository _animeRoleRepository;
		private readonly ISeasonRepository _seasonRepository;
		private readonly IMalApiService _malApiService;

		public UpdateSeiyuuHandler(
			ISeiyuuRepository seiyuuRepository,
			IAnimeRepository animeRepository,
			ICharacterRepository characterRepository,
			ISeiyuuRoleRepository seiyuuRoleRepository,
			IAnimeRoleRepository animeRoleRepository,
			ISeasonRepository seasonRepository,
			IMalApiService malApiService
		)
		{
			_seiyuuRepository = seiyuuRepository;
			_animeRepository = animeRepository;
			_characterRepository = characterRepository;
			_seiyuuRoleRepository = seiyuuRoleRepository;
			_animeRoleRepository = animeRoleRepository;
			_seasonRepository = seasonRepository;
			_malApiService = malApiService;
		}

		public async Task HandleAsync(UpdateSeiyuuMessage updateSeiyuuMessage)
		{
			var seiyuuToUpdate = await _seiyuuRepository.GetAsync(updateSeiyuuMessage.MalId);

			if (seiyuuToUpdate == null)
			{
				return;
			}

			var updateData = await _malApiService.GetSeiyuuDataAsync(updateSeiyuuMessage.MalId);

			if (updateData == null)
			{
				return;
			}

			UpdatePerson(seiyuuToUpdate, updateData);
			await _seiyuuRepository.UpdateAsync(seiyuuToUpdate);

			var rolesUpdateDate = await _malApiService.GetSeiyuuVoiceActingRolesAsync(updateSeiyuuMessage.MalId);

			await UpdateRolesAsync(updateSeiyuuMessage, rolesUpdateDate);
		}

		private void UpdatePerson(Seiyuu seiyuuToUpdate, MalSeiyuuUpdateData updateData)
		{
			seiyuuToUpdate.Name = updateData.Name;
			seiyuuToUpdate.About = updateData.About;
			seiyuuToUpdate.KanjiName = updateData.JapaneseName;
			seiyuuToUpdate.ImageUrl = updateData.ImageUrl;
			seiyuuToUpdate.Popularity = updateData.Popularity;
			seiyuuToUpdate.Birthday = updateData.Birthday;
		}

		private async Task UpdateRolesAsync(UpdateSeiyuuMessage updateSeiyuuMessage, ICollection<MalVoiceActingRoleUpdateData> parsedVoiceActingRoles)
		{
			var seiyuuExistingRoles = await _seiyuuRoleRepository.GetAllSeiyuuRolesAsync(updateSeiyuuMessage.Id, false);

			foreach (var parsedRole in parsedVoiceActingRoles)
			{
				if (!seiyuuExistingRoles.Any(role => role.Anime.MalId.Equals(parsedRole.AnimeMalId) && role.Character.MalId.Equals(parsedRole.CharacterMaId)))
				{
					var animeInDatabase = await InsertAnimeAsync(parsedRole.AnimeMalId);
					var characterInDatabase = await InsertCharacterAsync(parsedRole.CharacterMaId);

					if (animeInDatabase != null && characterInDatabase != null)
					{
						await _animeRoleRepository.AddAsync(new AnimeRole
						{
							Id = Guid.NewGuid(),
							LanguageId = LanguageId.Japanese,
							RoleTypeId = parsedRole.RoleType.Equals("Main") ? AnimeRoleTypeId.Main : AnimeRoleTypeId.Supporting,
							AnimeId = animeInDatabase.Id,
							CharacterId = characterInDatabase.Id,
							SeiyuuId = updateSeiyuuMessage.Id
						});
					}
				}
			}
		}

		private async Task<AnimeCharacter> InsertCharacterAsync(long malId)
		{
			var existingCharacter = await _characterRepository.GetAsync(malId);

			if (existingCharacter != null)
			{
				return existingCharacter;
			}

			var parsedData = await _malApiService.GetCharacterDataAsync(malId);

			if (parsedData is null)
			{
				return null;
			}

			var newCharacter = new AnimeCharacter
			{
				Id = Guid.NewGuid(),
				MalId = malId,
				ImageUrl = parsedData.ImageUrl,
				Name = parsedData.Name,
				Popularity = parsedData.Popularity,
				About = parsedData.About,
				KanjiName = parsedData.JapaneseName,
				Nicknames = parsedData.Nicknames
			};

			await _characterRepository.AddAsync(newCharacter);

			return newCharacter;
		}

		private async Task<Anime> InsertAnimeAsync(long malId)
		{
			var existingAnime = await _animeRepository.GetAsync(malId);

			if (existingAnime != null)
			{
				return existingAnime;
			}

			var parsedData = await _malApiService.GetAnimeDataAsync(malId);

			if (parsedData is null)
			{
				return null;
			}

			var newAnime = new Anime
			{
				Id = Guid.NewGuid(),
				MalId = malId,
				Title = parsedData.Title,
				ImageUrl = parsedData.ImageUrl,
				About = parsedData.About,
				AiringDate = parsedData.AiringDate ?? DateTime.MinValue,
				EnglishTitle = parsedData.EnglishTitle,
				KanjiTitle = parsedData.JapaneseTitle,
				Popularity = parsedData.Popularity,
				TitleSynonyms = !string.IsNullOrWhiteSpace(parsedData.TitleSynonyms) ? parsedData.TitleSynonyms : string.Empty,
				StatusId = JikanParserHelper.GetUpdatedAnimeStatus(null, parsedData.Status),
				TypeId = JikanParserHelper.GetUpdatedAnimeType(null, parsedData.Type),
				SeasonId = string.IsNullOrEmpty(parsedData.SeasonName)
				? await MatchSeasonByDateAsync(parsedData.AiringDate)
				: await MatchSeasonBySeasonAsync(parsedData.SeasonName, parsedData.SeasonYear)
			};

			await _animeRepository.AddAsync(newAnime);

			return newAnime;
		}

		private async Task<long?> MatchSeasonBySeasonAsync(string seasonName, int? seasonYear)
		{
			if (seasonYear is null)
			{
				return null;
			}

			var foundSeason = await _seasonRepository.GetAsync(x => x.Name.ToLower().Equals(seasonName.ToLower()) && x.Year.Equals(seasonYear.Value));
			return foundSeason?.Id;
		}

		private async Task<long?> MatchSeasonByDateAsync(DateTime? airingDate)
		{
			(string, int)? season = JikanParserHelper.GetSeasonPartsByAiringDate(airingDate);

			if (season is null)
			{
				return null;
			}

			var foundSeason = await _seasonRepository.GetAsync(x => x.Name.ToLower().Equals(season.Value.Item1.ToLower()) && x.Year.Equals(season.Value.Item2));
			return foundSeason?.Id;
		}
	}
}