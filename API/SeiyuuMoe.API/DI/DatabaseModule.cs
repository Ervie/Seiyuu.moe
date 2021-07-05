using Autofac;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Blacklisting;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;

namespace SeiyuuMoe.API.DI
{
	public class DatabaseModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterType<SeiyuuMoeContext>().AsSelf();

			builder.RegisterType<AnimeRepository>().AsImplementedInterfaces();
			builder.RegisterType<AnimeRoleRepository>().AsImplementedInterfaces();
			builder.RegisterType<BlacklistedIdRepository>().AsImplementedInterfaces();
			builder.RegisterType<CharacterRepository>().AsImplementedInterfaces();
			builder.RegisterType<SeasonRepository>().AsImplementedInterfaces();
			builder.RegisterType<SeasonRoleRepository>().AsImplementedInterfaces();
			builder.RegisterType<SeiyuuRepository>().AsImplementedInterfaces();
			builder.RegisterType<SeiyuuRoleRepository>().AsImplementedInterfaces();

			base.Load(builder);
		}
	}
}