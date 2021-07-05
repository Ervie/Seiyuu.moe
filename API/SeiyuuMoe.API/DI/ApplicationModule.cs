using Autofac;
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

namespace SeiyuuMoe.Application
{
	public class ApplicationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SeiyuuSearchCriteriaService>()
				.AsImplementedInterfaces();
			builder.RegisterType<AnimeSearchCriteriaService>()
				.AsImplementedInterfaces();
			builder.RegisterType<SeasonSearchCriteriaService>()
				.AsImplementedInterfaces();

			builder.RegisterType<SearchSeiyuuQueryHandler>()
				.AsSelf();
			builder.RegisterType<GetSeiyuuCardInfoQueryHandler>()
				.AsSelf();
			builder.RegisterType<CompareSeiyuuQueryHandler>()
				.AsSelf();
			builder.RegisterType<GetAnimeCardInfoQueryHandler>()
				.AsSelf();
			builder.RegisterType<SearchAnimeQueryHandler>()
				.AsSelf();
			builder.RegisterType<CompareAnimeQueryHandler>()
				.AsSelf();
			builder.RegisterType<GetSeasonSummariesQueryHandler>()
				.AsSelf();

			base.Load(builder);
		}
	}
}