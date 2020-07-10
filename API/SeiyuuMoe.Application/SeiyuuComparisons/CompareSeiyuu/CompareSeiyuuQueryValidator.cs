using FluentValidation;

namespace SeiyuuMoe.Application.SeiyuuComparisons.CompareSeiyuu
{
	public class CompareSeiyuuQueryValidator : AbstractValidator<CompareSeiyuuQuery>
	{
		public CompareSeiyuuQueryValidator()
		{
			RuleFor(x => x).NotNull();
			RuleFor(x => x.SeiyuuMalIds).NotNull();
			RuleFor(x => x.SeiyuuMalIds).Must(y => y.Count >= 2);
		}
	}
}