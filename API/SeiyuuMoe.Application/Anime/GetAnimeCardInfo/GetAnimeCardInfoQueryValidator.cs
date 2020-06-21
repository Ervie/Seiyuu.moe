using FluentValidation;

namespace SeiyuuMoe.Application.Anime.GetAnimeCardInfo
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