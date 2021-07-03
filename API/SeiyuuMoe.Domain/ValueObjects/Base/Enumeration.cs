using System;

namespace SeiyuuMoe.Domain.ValueObjects.Base
{
	public abstract class Enumeration : IComparable, IEquatable<Enumeration>
	{
		public string Value { get; }

		public string DisplayName { get; }

		protected Enumeration(string value, string displayName)
		{
			Value = value;
			DisplayName = displayName;
		}

		public static implicit operator string(Enumeration enumeration) => enumeration.Value;

		public override string ToString() => DisplayName;

		public override bool Equals(object? obj)
		{
			if (!(obj is Enumeration otherValue))
			{
				return false;
			}

			var typeMatches = GetType() == obj.GetType();
			var valueMatches = Value.Equals(otherValue.Value);

			return typeMatches && valueMatches;
		}

		public int CompareTo(object? other) => String.Compare(Value, ((Enumeration)other!).Value, StringComparison.Ordinal);

		public bool Equals(Enumeration? other)
		{
			if (other is null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			return Value == other.Value && DisplayName == other.DisplayName;
		}

		public override int GetHashCode() => unchecked((Value.GetHashCode() * 397) ^ DisplayName.GetHashCode());
	}
}