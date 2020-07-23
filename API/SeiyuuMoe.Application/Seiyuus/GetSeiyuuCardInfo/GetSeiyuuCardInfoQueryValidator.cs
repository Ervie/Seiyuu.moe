using FluentValidation;

namespace SeiyuuMoe.Application.Seiyuus.GetSeiyuuCardInfo
{
	public class GetSeiyuuCardInfoQueryValidator : AbstractValidator<GetSeiyuuCardInfoQuery>
	{
		public GetSeiyuuCardInfoQueryValidator()
		{
			RuleFor(x => x.MalId)
				.GreaterThanOrEqualTo(1);
		}
	}
}