using SeiyuuMoe.Application.Seasons.GetSeasonSummaries;

namespace SeiyuuMoe.Tests.Unit.Builders.Queries
{
	internal class GetSeasonSummariesQueryBuilder
	{
		private bool _tvSeriesOnly;
		private long _id;

		public GetSeasonSummariesQuery Build() => new GetSeasonSummariesQuery
		{
			TVSeriesOnly = _tvSeriesOnly,
			Id = _id
		};

		public GetSeasonSummariesQueryBuilder WithTvSeriesOnly(bool tvSeriesOnly)
		{
			_tvSeriesOnly = tvSeriesOnly;
			return this;
		}

		public GetSeasonSummariesQueryBuilder WithId(long id)
		{
			_id = id;
			return this;
		}
	}
}