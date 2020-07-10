using FluentValidation;

namespace SeiyuuMoe.Application.Seasons.GetSeasonSummaries
{
	public class GetSeasonSummariesQueryValidator : AbstractValidator<GetSeasonSummariesQuery>
	{
		public GetSeasonSummariesQueryValidator()
		{
			RuleFor(x => x).NotNull();
			RuleFor(x => x.Year).GreaterThanOrEqualTo(1916);
			RuleFor(x => x.Season).Must(y => !string.IsNullOrWhiteSpace(y));
		}
	}
}