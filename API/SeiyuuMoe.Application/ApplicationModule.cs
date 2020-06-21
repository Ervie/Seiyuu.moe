using Autofac;
using SeiyuuMoe.Application.Anime;
using SeiyuuMoe.Application.Anime.GetAnimeCardInfo;
using SeiyuuMoe.Application.Anime.SearchAnime;
using SeiyuuMoe.Application.AnimeComparison.CompareAnime;
using SeiyuuMoe.Application.Season;
using SeiyuuMoe.Application.Season.GetSeasonSummaries;
using SeiyuuMoe.Application.Seiyuu;
using SeiyuuMoe.Application.Seiyuu.GetSeiyuuCardInfo;
using SeiyuuMoe.Application.Seiyuu.SearchSeiyuu;
using SeiyuuMoe.Application.SeiyuuComparison.CompareSeiyuu;

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