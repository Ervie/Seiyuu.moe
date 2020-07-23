using FluentValidation;

namespace SeiyuuMoe.Application.Animes.GetAnimeCardInfo
{
	internal class GetAnimeCardInfoQueryValidator : AbstractValidator<GetAnimeCardInfoQuery>
	{
		public GetAnimeCardInfoQueryValidator()
		{
			RuleFor(x => x.MalId)
				.GreaterThanOrEqualTo(1);
		}
	}
}