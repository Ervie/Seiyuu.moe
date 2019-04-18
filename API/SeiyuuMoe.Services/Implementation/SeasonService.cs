﻿using EnsureThat;
using SeiyuuMoe.BusinessServices;
using SeiyuuMoe.Contracts.Dtos.Season;
using SeiyuuMoe.Contracts.SearchCriteria;
using SeiyuuMoe.Services.Interfaces;
using SeiyuuMoe.WebEssentials;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Services.Implementation
{
	internal class SeasonService : ISeasonService
	{
		private ISeasonBusinessService seasonBusinessService;

		public SeasonService(ISeasonBusinessService seasonBusinessService)
		{
			this.seasonBusinessService = seasonBusinessService;
		}

		public async Task<QueryResponse<ICollection<SeasonSummaryEntryDto>>> GetSeasonSummary(Query<SeasonSearchCriteria> query)
		{
			Ensure.That(query, nameof(query)).IsNotNull();
			Ensure.That(query.SearchCriteria, nameof(query.SearchCriteria)).IsNotNull();
			Ensure.That(query.SearchCriteria.Year).IsGte(1916);
			Ensure.That(query.SearchCriteria.Season).IsNotEmptyOrWhitespace();

			return new QueryResponse<ICollection<SeasonSummaryEntryDto>>(await seasonBusinessService.GetSeasonRolesSummary(query));
		}
	}
}