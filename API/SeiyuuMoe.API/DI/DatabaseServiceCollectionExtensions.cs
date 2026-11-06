using Microsoft.Extensions.DependencyInjection;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Database.Animes;
using SeiyuuMoe.Infrastructure.Database.Characters;
using SeiyuuMoe.Infrastructure.Database.Context;
using SeiyuuMoe.Infrastructure.Database.Seasons;
using SeiyuuMoe.Infrastructure.Database.Seiyuus;

namespace SeiyuuMoe.API.DI
{
	public static class DatabaseServiceCollectionExtensions
	{
		public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
		{
			services.AddTransient<SeiyuuMoeContext>();

			services.AddTransient<IAnimeRepository, AnimeRepository>();
			services.AddTransient<IAnimeRoleRepository, AnimeRoleRepository>();
			services.AddTransient<ICharacterRepository, CharacterRepository>();
			services.AddTransient<ISeasonRepository, SeasonRepository>();
			services.AddTransient<ISeasonRoleRepository, SeasonRoleRepository>();
			services.AddTransient<ISeiyuuRepository, SeiyuuRepository>();
			services.AddTransient<ISeiyuuRoleRepository, SeiyuuRoleRepository>();

			return services;
		}
	}
}
