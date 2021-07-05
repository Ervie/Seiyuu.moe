using SeiyuuMoe.Domain.Validation;
using System;

namespace SeiyuuMoe.Domain.ValueObjects.Base
{
	public abstract class StringValueBase : IEquatable<StringValueBase>
	{
		public string Value { get; }

		protected StringValueBase(string value)
		{
			Guard.IsNotNullOrWhiteSpace(value, nameof(value));
			Value = value;
		}

		public override bool Equals(object obj)
		{
			if (obj is null)
			{
				return false;
			}

			return obj is StringValueBase other && Equals(other);
		}

		public override int GetHashCode() => Value.GetHashCode();

		public bool Equals(StringValueBase other) => other != null && Value == other.Value;

		public static bool operator ==(StringValueBase obj1, StringValueBase obj2) => obj1?.Equals(obj2) ?? Equals(obj2, null);

		public static bool operator !=(StringValueBase obj1, StringValueBase obj2) => !(obj1 == obj2);
	}
}