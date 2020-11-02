using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.MalBackgroundJobs.Application.Helpers;
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
			var animeToUpdate = await _animeRepository.GetAsync(updateAnimeMessage.MalId);

			if (animeToUpdate == null)
			{
				return;
			}

			var updateData = await _malApiService.GetAnimeDataAsync(updateAnimeMessage.MalId);

			if (updateData == null)
			{
				return;
			}

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
			animeToUpdate.TitleSynonyms = !string.IsNullOrWhiteSpace(malAnimeUpdateData.TitleSynonyms) ? malAnimeUpdateData.TitleSynonyms : animeToUpdate.TitleSynonyms;
			animeToUpdate.StatusId = JikanParserHelper.GetUpdatedAnimeStatus(animeToUpdate.StatusId, malAnimeUpdateData.Status);
			animeToUpdate.TypeId = JikanParserHelper.GetUpdatedAnimeType(animeToUpdate.TypeId, malAnimeUpdateData.Type);
			animeToUpdate.SeasonId = string.IsNullOrEmpty(malAnimeUpdateData.Season)
				? await MatchSeasonByDateAsync(malAnimeUpdateData.AiringDate)
				: await MatchSeasonBySeasonAsync(malAnimeUpdateData.Season);
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