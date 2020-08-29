using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.Animes;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Infrastructure.Animes;
using SeiyuuMoe.Infrastructure.Context;
using SeiyuuMoe.Tests.Common.Helpers;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Component.Animes
{
	public class SearchAnimeQueryHandlerTests
	{
		[Fact]
		public async Task HandleAsync_GivenNoMatchingAnime_ShouldReturnEmptyResults()
		{
			// Given
			var query = new SearchAnimeQuery();
			var dbContext = InMemoryDbProvider.GetDbContext();
			var handler = CreateHandler(dbContext);

			// When
			var result = await handler.HandleAsync(query);

			// Then
			using (new AssertionScope())
			{
				//result.Found.Should().BeTrue();
				result.Payload.Results.Should().BeEmpty();
			}
		}

		private SearchAnimeQueryHandler CreateHandler(SeiyuuMoeContext seiyuuMoeContext)
		{
			var animeRepository = new AnimeRepository(seiyuuMoeContext);
			var animeSearchCriteriaService = new AnimeSearchCriteriaService();

			return new SearchAnimeQueryHandler(animeRepository, animeSearchCriteriaService);
		}
	}
}