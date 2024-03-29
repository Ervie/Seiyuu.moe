﻿using System.Collections.Generic;

namespace SeiyuuMoe.Application.Seasons.GetSeasonSummaries
{
	public class GetSeasonSummariesQuery
	{
		public long Year { get; set; }

		public string Season { get; set; }

		public bool MainRolesOnly { get; set; }

		public bool TVSeriesOnly { get; set; }

		public long? Id { get; set; }

		public int Page { get; set; }

		public int PageSize { get; set; }
	}
}