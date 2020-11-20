using JikanDotNet;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Characters;
using SeiyuuMoe.Infrastructure.Configuration;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Infrastructure.Jikan;
using SeiyuuMoe.Infrastructure.S3;
using SeiyuuMoe.Infrastructure.Seasons;
using SeiyuuMoe.Infrastructure.Seiyuus;
using SeiyuuMoe.MalBackgroundJobs.Application.Handlers;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Base;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda.Function
{
	public class InsertSeiyuuLambda : BaseLambda
	{
		protected async override Task HandleAsync()
		{
			Console.WriteLine($"InsertSeiyuuLambda was invoked.");

			var dbConfig = ConfigurationReader.DatabaseConfiguration;
			using var dbContext = new SeiyuuMoeContext(dbConfig);

			var handler = CreateHandler(dbContext);
			await handler.HandleAsync();
		}

		private static InsertSeiyuuHandler CreateHandler(SeiyuuMoeContext dbContext)
		{
			var scheduleConfiguration = ConfigurationReader.MalBgJobsScheduleConfiguration;

			var animeRepository = new AnimeRepository(dbContext);
			var seiyuuRepository = new SeiyuuRepository(dbContext);
			var characterRepository = new CharacterRepository(dbContext);
			var animeRoleRepository = new AnimeRoleRepository(dbContext);
			var seasonRepository = new SeasonRepository(dbContext);

			var jikanUrl = ConfigurationReader.JikanUrl;
			var jikanClient = new Jikan(jikanUrl, true);
			var jikanService = new JikanService(jikanClient);

			var s3Client = new S3Service();

			return new InsertSeiyuuHandler(
				scheduleConfiguration.DelayBetweenMessagesInSeconds,
				seiyuuRepository,
				seasonRepository,
				characterRepository,
				animeRepository,
				animeRoleRepository,
				jikanService,
				s3Client
			);
		}
	}
}