using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu;
using SeiyuuMoe.Application.Seiyuus.SearchSeiyuu;
using SeiyuuMoe.Tests.E2E.Apis;
using Xunit;

namespace SeiyuuMoe.Tests.E2E.Tests;

public class SeiyuuControllerTests: BaseSetupClass
{
		[Fact]
		public async Task GetCardInfo_ExistingSeiyuu_ShouldReturnCardInfo()
		{
			// Given
			const int sekiTomokazuId = 1;
			var api = new SeiyuuApi(requester);

			// When
			var apiResult = await api.GetCardInfoAsync(sekiTomokazuId);

			// Then
			using var scope = new AssertionScope();
			apiResult.Name.Should().Be("Tomokazu Seki");
			apiResult.JapaneseName.Should().Be("関 智一");
			apiResult.ImageUrl.Should().NotBeNullOrWhiteSpace();
			apiResult.MalId.Should().Be(sekiTomokazuId);
			apiResult.About.Should().NotBeNullOrWhiteSpace();
		}

		[Fact]
		public async Task GetCardInfo_NotExistingSeiyuu_ShouldReturnEmptyInfo()
		{
			// Given
			const int wrongId = int.MaxValue;
			var api = new SeiyuuApi(requester);

			// When
			var apiResult = await api.GetCardInfoAsync(wrongId);

			// Then
			apiResult.Should().BeNull();
		}

		[Fact]
		public async Task GetSearchEntries_ExistingSeiyuu_ShouldReturnSearchEntries()
		{
			// Given
			const int sekiTomokazuId = 1;
			var api = new SeiyuuApi(requester);

			var query = new SearchSeiyuuQuery()
			{
				Name = "Seki"
			};

			// When
			var apiResult = await api.GetSearchEntriesAsync(query);

			// Then
			using var _ = new AssertionScope();
			var firstResult = apiResult.Results.First();

			apiResult.Results.Count.Should().BeGreaterOrEqualTo(10);
			firstResult.Name.Should().Be("Tomokazu Seki");
			firstResult.ImageUrl.Should().NotBeNullOrWhiteSpace();
			firstResult.MalId.Should().Be(sekiTomokazuId);
		}

		[Fact]
		public async Task GetSearchEntries_NotExistingSeiyuu_ShouldReturnEmptyCollection()
		{
			// Given
			var api = new SeiyuuApi(requester);

			var query = new SearchSeiyuuQuery
			{
				Name = "qwertyuiopasdghjkl"
			};

			// When
			var apiResult = await api.GetSearchEntriesAsync(query);

			// Then
			apiResult.Results.Should().BeEmpty();
		}

		[Fact]
		public async Task GetComparison_ExistingSeiyuus_ShouldReturnComparisonEntities()
		{
			// Given
			const int sekiMalId = 1;
			const int sugitaMalId = 2;
			var api = new SeiyuuApi(requester);

			var query = new CompareSeiyuuQuery
			{
				SeiyuuMalIds = new List<long> { sekiMalId, sugitaMalId }
			};

			// When
			var apiResult = await api.GetComparison(query);

			// Then
			apiResult.Should().HaveCountGreaterOrEqualTo(25);
		}
}