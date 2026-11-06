using Microsoft.Extensions.DependencyInjection;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.GetAnimeCardInfo;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Application.AnimeComparisons.CompareAnime;
using SeiyuuMoe.Application.Seasons;
using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;
using SeiyuuMoe.Application.Seiyuus;
using SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu;

namespace SeiyuuMoe.API.DI
{
	public static class ApplicationServiceCollectionExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services)
		{
			services.AddTransient<ISeiyuuSearchCriteriaService, SeiyuuSearchCriteriaService>();
			services.AddTransient<IAnimeSearchCriteriaService, AnimeSearchCriteriaService>();
			services.AddTransient<ISeasonSearchCriteriaService, SeasonSearchCriteriaService>();

			services.AddTransient<SearchSeiyuuQueryHandler>();
			services.AddTransient<GetSeiyuuCardInfoQueryHandler>();
			services.AddTransient<CompareSeiyuuQueryHandler>();
			services.AddTransient<GetAnimeCardInfoQueryHandler>();
			services.AddTransient<SearchAnimeQueryHandler>();
			services.AddTransient<CompareAnimeQueryHandler>();
			services.AddTransient<GetSeasonSummariesQueryHandler>();

			return services;
		}
	}
}
