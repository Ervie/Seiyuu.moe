using SeiyuuMoe.Domain.Validation;

namespace SeiyuuMoe.Domain.ValueObjects
{
	public class Title
	{
		public string Value { get; }

		public Title(string value)
		{
			Guard.IsNotNullOrWhiteSpace(value, nameof(value));
			Value = value;
		}
	}
}