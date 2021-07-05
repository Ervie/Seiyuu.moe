using SeiyuuMoe.Domain.Validation;
using System;

namespace SeiyuuMoe.Domain.ValueObjects.Base
{
	public class NumericalId : IEquatable<NumericalId>
	{
		public long Value { get; }

		public NumericalId(long value)
		{
			Guard.IsPositive(value, nameof(value));
			Value = value;
		}

		public override string ToString() => Value.ToString();

		public bool Equals(NumericalId? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Value == other.Value;
		}

		public override bool Equals(object? obj)
		{
			if (obj is null)
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			if (obj.GetType() != GetType())
			{
				return false;
			}

			return Equals((NumericalId)obj);
		}

		public override int GetHashCode() => Value.GetHashCode();
	}
}