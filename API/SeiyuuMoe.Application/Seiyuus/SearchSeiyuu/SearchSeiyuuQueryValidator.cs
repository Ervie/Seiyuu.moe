using FluentValidation;

namespace SeiyuuMoe.Application.Seiyuus.SearchSeiyuu
{
	public class SearchSeiyuuQueryValidator : AbstractValidator<SearchSeiyuuQuery>
	{
		public SearchSeiyuuQueryValidator()
		{
			RuleFor(x => x).NotNull();
		}
	}
}