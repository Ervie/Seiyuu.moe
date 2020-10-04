using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class UpdateAnimeHandler
	{
		private readonly IAnimeRepository _animeRepository;
		//private readonly ISeasonRepository _seasonRepository;
		private readonly IMalApiService _malApiService;

		public UpdateAnimeHandler(IAnimeRepository animeRepository, IMalApiService malApiService)
		{
			_animeRepository = animeRepository;
			_malApiService = malApiService;
		}

		public async Task HandleAsync(UpdateAnimeMessage updateAnimeMessage)
		{
			var updateData = await _malApiService.GetAnimeDataAsync(updateAnimeMessage.MalId);
			var animeToUpdate = await _animeRepository.GetAsync(updateAnimeMessage.MalId);
			UpdateAnime(animeToUpdate, updateData);
			await _animeRepository.UpdateAsync(animeToUpdate);
		}

		private static void UpdateAnime(Anime animeToUpdate, MalAnimeUpdateData malAnimeUpdateData)
		{
			animeToUpdate.Title = malAnimeUpdateData.Title;
			animeToUpdate.About = malAnimeUpdateData.About;
			animeToUpdate.EnglishTitle = malAnimeUpdateData.EnglishTitle;
			animeToUpdate.KanjiTitle = malAnimeUpdateData.JapaneseTitle;
			animeToUpdate.Popularity = malAnimeUpdateData.Popularity;
			animeToUpdate.ImageUrl = malAnimeUpdateData.ImageUrl;
			animeToUpdate.AiringDate = malAnimeUpdateData.AiringDate ?? animeToUpdate.AiringDate;
			animeToUpdate.TitleSynonyms = string.IsNullOrWhiteSpace(malAnimeUpdateData.TitleSynonyms) ? malAnimeUpdateData.TitleSynonyms : animeToUpdate.TitleSynonyms;
		}
	}
}