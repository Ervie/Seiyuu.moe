using FluentValidation;

namespace SeiyuuMoe.Application.Animes.SearchAnime
{
	internal class SearchAnimeQueryValidator : AbstractValidator<SearchAnimeQuery>
	{
		public SearchAnimeQueryValidator()
		{
			RuleFor(x => x).NotNull();
		}
	}
}