using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.MalBackgroundJobs.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class InsertSeiyuuHandler
	{
		private readonly int _insertSeiyuuBatchSize;
		private readonly int _delayBetweenCallsInSeconds;
		private readonly ISeiyuuRepository _seiyuuRepository;
		private readonly ISeasonRepository _seasonRepository;
		private readonly ICharacterRepository _characterRepository;
		private readonly IAnimeRepository _animeRepository;
		private readonly IAnimeRoleRepository _animeRoleRepository;
		private readonly IMalApiService _malApiService;
		private readonly IS3Service _s3Service;

		public InsertSeiyuuHandler(
			int insertSeiyuuBatchSize,
			int delayBetweenCallsInSeconds,
			ISeiyuuRepository seiyuuRepository,
			ISeasonRepository seasonRepository,
			ICharacterRepository characterRepository,
			IAnimeRepository animeRepository,
			IAnimeRoleRepository animeRoleRepository,
			IMalApiService malApiService,
			IS3Service s3Service
		)
		{
			_insertSeiyuuBatchSize = insertSeiyuuBatchSize;
			_delayBetweenCallsInSeconds = delayBetweenCallsInSeconds;
			_seiyuuRepository = seiyuuRepository;
			_seasonRepository = seasonRepository;
			_characterRepository = characterRepository;
			_animeRepository = animeRepository;
			_animeRoleRepository = animeRoleRepository;
			_malApiService = malApiService;
			_s3Service = s3Service;
		}

		public async Task HandleAsync()
		{
			var bgJobsState = await _s3Service.GetBgJobsStateAsync(Environment.GetEnvironmentVariable("JobsStateBucket"));
			var lastSeiyuuId = bgJobsState.LastCheckedSeiyuuMalId;

			var seiyuuUpdateMessages = Enumerable
				.Range(lastSeiyuuId + 1, _insertSeiyuuBatchSize)
				.Select(i => new UpdateSeiyuuMessage { Id = Guid.NewGuid(), MalId = i });

			foreach (var seiyuuUpdateMessage in seiyuuUpdateMessages)
			{
				await Task.Delay(_delayBetweenCallsInSeconds * 1000);
				Console.WriteLine($"Checking malId {seiyuuUpdateMessage.MalId}");
				await InsertSingleSeiyuu(seiyuuUpdateMessage);
			}

			bgJobsState.LastCheckedSeiyuuMalId += _insertSeiyuuBatchSize;
			await _s3Service.PutBgJobsStateAsync(Environment.GetEnvironmentVariable("JobsStateBucket"), bgJobsState);
		}

		public async Task InsertSingleSeiyuu(UpdateSeiyuuMessage updateSeiyuuMessage)
		{
			var seiyuuToUpdate = await _seiyuuRepository.GetAsync(updateSeiyuuMessage.MalId);

			if (seiyuuToUpdate != null)
			{
				return;
			}

			var updateData = await _malApiService.GetSeiyuuDataAsync(updateSeiyuuMessage.MalId);

			if (updateData == null || !JikanParserHelper.IsJapanese(updateData.JapaneseName) || !updateData.VoiceActingRoles.Any())
			{
				return;
			}

			await InsertSeiyuuAsync(updateSeiyuuMessage, updateData);

			await InsertRolesAsync(updateSeiyuuMessage, updateData.VoiceActingRoles);
		}

		private async Task InsertSeiyuuAsync(UpdateSeiyuuMessage updateSeiyuuMessage, MalSeiyuuUpdateData updateData)
		{
			var newSeiyuu = new Seiyuu
			{
				Id = updateSeiyuuMessage.Id,
				MalId = updateSeiyuuMessage.MalId,
				Name = updateData.Name,
				About = updateData.About,
				KanjiName = updateData.JapaneseName,
				ImageUrl = updateData.ImageUrl,
				Popularity = updateData.Popularity,
				Birthday = updateData.Birthday,
			};

			Console.WriteLine($"Inserted seiyuu with malId {updateSeiyuuMessage.MalId} ({updateData.Name})");

			await _seiyuuRepository.AddAsync(newSeiyuu);
		}

		private async Task InsertRolesAsync(UpdateSeiyuuMessage updateSeiyuuMessage, ICollection<MalVoiceActingRoleUpdateData> parsedVoiceActingRoles)
		{
			foreach (var parsedRole in parsedVoiceActingRoles)
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

			Console.WriteLine($"Inserted character with malId {malId} ({parsedData.Name})");

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
				SeasonId = string.IsNullOrEmpty(parsedData.Season)
				? await MatchSeasonByDateAsync(parsedData.AiringDate)
				: await MatchSeasonBySeasonAsync(parsedData.Season)
			};

			await _animeRepository.AddAsync(newAnime);

			Console.WriteLine($"Inserted anime with malId {malId} ({parsedData.Title})");

			return newAnime;
		}

		private async Task<long?> MatchSeasonBySeasonAsync(string seasonName)
		{
			(string, int)? season = JikanParserHelper.GetSeasonPartsByName(seasonName);

			if (season is null)
			{
				return null;
			}

			var foundSeason = await _seasonRepository.GetAsync(x => x.Name.ToLower().Equals(season.Value.Item1.ToLower()) && x.Year.Equals(season.Value.Item2));
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