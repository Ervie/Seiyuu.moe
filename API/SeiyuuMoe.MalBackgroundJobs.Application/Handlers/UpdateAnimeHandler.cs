using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class UpdateAnimeHandler
	{
		private readonly IAnimeRepository _animeRepository;
		private readonly ISeasonRepository _seasonRepository;
		private readonly IMalApiService _malApiService;

		public UpdateAnimeHandler(IAnimeRepository animeRepository, ISeasonRepository seasonRepository, IMalApiService malApiService)
		{
			_animeRepository = animeRepository;
			_seasonRepository = seasonRepository;
			_malApiService = malApiService;
		}

		public async Task HandleAsync(UpdateAnimeMessage updateAnimeMessage)
		{
			var updateData = await _malApiService.GetAnimeDataAsync(updateAnimeMessage.MalId);
			var animeToUpdate = await _animeRepository.GetAsync(updateAnimeMessage.MalId);
			await UpdateAnimeAsync(animeToUpdate, updateData);
			await _animeRepository.UpdateAsync(animeToUpdate);
		}

		private async Task UpdateAnimeAsync(Anime animeToUpdate, MalAnimeUpdateData malAnimeUpdateData)
		{
			animeToUpdate.Title = malAnimeUpdateData.Title;
			animeToUpdate.About = malAnimeUpdateData.About;
			animeToUpdate.EnglishTitle = malAnimeUpdateData.EnglishTitle;
			animeToUpdate.KanjiTitle = malAnimeUpdateData.JapaneseTitle;
			animeToUpdate.Popularity = malAnimeUpdateData.Popularity;
			animeToUpdate.ImageUrl = malAnimeUpdateData.ImageUrl;
			animeToUpdate.AiringDate = malAnimeUpdateData.AiringDate ?? animeToUpdate.AiringDate;
			animeToUpdate.TitleSynonyms = string.IsNullOrWhiteSpace(malAnimeUpdateData.TitleSynonyms) ? malAnimeUpdateData.TitleSynonyms : animeToUpdate.TitleSynonyms;
			animeToUpdate.StatusId = UpdateAnimeStatus(animeToUpdate.StatusId, malAnimeUpdateData.Status);
			animeToUpdate.TypeId = UpdateAnimeType(animeToUpdate.TypeId, malAnimeUpdateData.Type);
			animeToUpdate.SeasonId = string.IsNullOrEmpty(malAnimeUpdateData.Season)
				? await MatchSeasonByDateAsync(malAnimeUpdateData.AiringDate)
				: await MatchSeasonBySeasonAsync(malAnimeUpdateData.Season);
		}

		private async Task<long?> MatchSeasonBySeasonAsync(string season)
		{
			var seasonParts = season.Split(' ');

			if (seasonParts.Length < 2)
				return null;

			var year = int.Parse(seasonParts[1]);
			var seasonName = seasonParts[0];

			var foundSeason = await _seasonRepository.GetAsync(x => x.Name.ToLower().Equals(seasonName.ToLower()) && x.Year.Equals(year));
			return foundSeason?.Id;
		}

		private async Task<long?> MatchSeasonByDateAsync(DateTime? airingDate)
		{
			if (!airingDate.HasValue)
				return null;

			var airingDay = airingDate.Value.Day;
			var airingYear = airingDate.Value.Year;

			var seasonName = airingDay switch
			{
				var day when day > 349 || day < 75 => "Winter",
				var day when day >= 75 && day < 166 => "Spring",
				var day when day >= 167 && day < 258 => "Summer",
				_ => "Fall"
			};

			if (airingDay > 349)
			{
				airingYear++;
			}

			var foundSeason = await _seasonRepository.GetAsync(x => x.Name.Equals(seasonName.ToString()) && x.Year.Equals(airingYear));
			return foundSeason?.Id;
		}

		private static AnimeStatusId? UpdateAnimeStatus(AnimeStatusId? currentStatus, string newStatus) =>
			Enum.TryParse(newStatus.Replace(" ", ""), out AnimeStatusId parsedAnimeStatus) ?
				parsedAnimeStatus :
				currentStatus;

		private static AnimeTypeId? UpdateAnimeType(AnimeTypeId? currentType, string newType) =>
			Enum.TryParse(newType, out AnimeTypeId parsedAnimeType) ?
				parsedAnimeType :
				currentType;
	}
}