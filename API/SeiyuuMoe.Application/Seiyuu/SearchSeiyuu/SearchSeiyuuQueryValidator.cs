using FluentValidation;

namespace SeiyuuMoe.Application.Seiyuu.SearchSeiyuu
{
	public class SearchSeiyuuQueryValidator : AbstractValidator<SearchSeiyuuQuery>
	{
		public SearchSeiyuuQueryValidator()
		{
			RuleFor(x => x).NotNull();
		}
	}
}