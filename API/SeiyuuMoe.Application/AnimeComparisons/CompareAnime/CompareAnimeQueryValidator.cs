using FluentValidation;

namespace SeiyuuMoe.Application.AnimeComparisons.CompareAnime
{
	public class CompareAnimeQueryValidator : AbstractValidator<CompareAnimeQuery>
	{
		public CompareAnimeQueryValidator()
		{
			RuleFor(x => x).NotNull();
			RuleFor(x => x.AnimeMalIds).NotNull();
			RuleFor(x => x.AnimeMalIds).Must(y => y.Count >= 2);
		}
	}
}