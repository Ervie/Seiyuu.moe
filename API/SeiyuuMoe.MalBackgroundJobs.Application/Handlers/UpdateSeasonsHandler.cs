using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class UpdateSeasonsHandler
	{
		private readonly ISeasonRepository _seasonRepository;
		private readonly IMalApiService _malApiService;

		public UpdateSeasonsHandler(ISeasonRepository seasonRepository, IMalApiService malApiService)
		{
			_seasonRepository = seasonRepository;
			_malApiService = malApiService;
		}

		public async Task HandleAsync()
		{
			var updateData = await _malApiService.GetSeasonDataAsync();

			if (updateData is null)
			{
				return;
			}

			var existingSeason = await _seasonRepository.GetAsync(season => season.Year == updateData.NewestSeasonYear && season.Name == updateData.NewestSeasonName);

			if (existingSeason is null)
			{
				var newSeason = new AnimeSeason
				{
					Name = updateData.NewestSeasonName,
					Year = updateData.NewestSeasonYear
				};

				await _seasonRepository.AddAsync(newSeason);
			}
		}
	}
}