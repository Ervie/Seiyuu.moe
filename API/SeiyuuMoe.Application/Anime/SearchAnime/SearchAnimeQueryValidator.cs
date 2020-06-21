using FluentValidation;

namespace SeiyuuMoe.Application.Anime.SearchAnime
{
	internal class SearchAnimeQueryValidator : AbstractValidator<SearchAnimeQuery>
	{
		public SearchAnimeQueryValidator()
		{
			RuleFor(x => x).NotNull();
		}
	}
}