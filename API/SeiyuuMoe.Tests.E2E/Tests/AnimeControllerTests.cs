using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.AnimeComparisons.CompareAnime;
using SeiyuuMoe.Application.Animes.SearchAnime;
using SeiyuuMoe.Tests.E2E;
using SeiyuuMoe.Tests.E2E.Apis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace SeiyuuMoe.Tests.Tests.E2E
{
	public class AnimeControllerTests : BaseSetupClass
	{
		[Fact]
		public async Task GetCardInfo_ExistingAnime_ShouldReturnCardInfo()
		{
			// Given
			const int cowboyBebopId = 1;
			var api = new AnimeApi(requester);

			// When
			var apiResult = await api.GetCardInfoAsync(cowboyBebopId);

			// Then
			using var scope = new AssertionScope();
			apiResult.Title.Should().Be("Cowboy Bebop");
			apiResult.Type.Should().Be("TV");
			apiResult.ImageUrl.Should().NotBeNullOrWhiteSpace();
			apiResult.MalId.Should().Be(cowboyBebopId);
			apiResult.Status.Should().Be("Finished Airing");
		}

		[Fact]
		public async Task GetCardInfo_NotExistingAnime_ShouldReturnEmptyInfo()
		{
			// Given
			const int wrongId = int.MaxValue;
			var api = new AnimeApi(requester);

			// When
			var apiResult = await api.GetCardInfoAsync(wrongId);

			// Then
			apiResult.Should().BeNull();
		}

		[Fact]
		public async Task GetSearchEntries_ExistingAnime_ShouldReturnSearchEntries()
		{
			// Given
			const int cowboyBebopId = 1;
			var api = new AnimeApi(requester);

			var query = new SearchAnimeQuery
			{
				Title = "Cowboy Bebop"
			};

			// When
			var apiResult = await api.GetSearchEntriesAsync(query);

			// Then
			using var scope = new AssertionScope();
			var firstResult = apiResult.Results.First();

			apiResult.Results.Count.Should().Be(4);
			firstResult.Title.Should().Be("Cowboy Bebop");
			firstResult.ImageUrl.Should().NotBeNullOrWhiteSpace();
			firstResult.MalId.Should().Be(cowboyBebopId);
		}

		[Fact]
		public async Task GetSearchEntries_NotExistingAnime_ShouldReturnEmptyCollection()
		{
			// Given
			var api = new AnimeApi(requester);

			var query = new SearchAnimeQuery
			{
				Title = "qwertyuiopasdghjkl"
			};

			// When
			var apiResult = await api.GetSearchEntriesAsync(query);

			// Then
			apiResult.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task GetComparison_ExistingAnime_ShouldReturnComparisonEntities()
		{
			// Given
			const int bleachId = 269;
			const int deathNoteId = 1535;
			var api = new AnimeApi(requester);

			var query = new CompareAnimeQuery
			{
				AnimeMalIds = new List<long> { bleachId, deathNoteId }
			};

			// When
			var apiResult = await api.GetComparison(query);

			// Then
			apiResult.Count.Should().Be(7);
		}
	}
}