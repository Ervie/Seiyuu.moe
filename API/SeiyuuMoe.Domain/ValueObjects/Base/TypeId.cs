using SeiyuuMoe.Domain.Validation;
using System;

namespace SeiyuuMoe.Domain.ValueObjects.Base
{
	public abstract class TypeId : IEquatable<TypeId>
	{
		public Guid Value { get; }

		protected TypeId(Guid value)
		{
			Guard.IsNotEmpty(value, nameof(TypeId));
			Value = value;
		}

		public bool Equals(TypeId? other)
		{
			if (other is null)
			{
				return false;
			}

			return ReferenceEquals(this, other) || Value == other.Value;
		}

		public override bool Equals(object? obj)
		{
			if (obj is null)
			{
				return false;
			}

			return ReferenceEquals(this, obj) || obj.GetType() == GetType() && Equals((TypeId)obj);
		}

		public override int GetHashCode() => Value.GetHashCode();

		public override string ToString() => Value.ToString();
	}
}