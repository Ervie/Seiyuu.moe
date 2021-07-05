using SeiyuuMoe.Domain.Validation;
using System;

namespace SeiyuuMoe.Domain.ValueObjects
{
	public class ReleaseDate
	{
		public DateTime Value { get; }

		public ReleaseDate(DateTime value)
		{
			Guard.IsDate(value, nameof(value));
			Guard.IsNotDefault(value, nameof(value));

			Value = value;
		}
	}
}