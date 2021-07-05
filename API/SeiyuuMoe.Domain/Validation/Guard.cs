using SeiyuuMoe.Domain.Exceptions;
using System;

namespace SeiyuuMoe.Domain.Validation
{
	public static class Guard
	{
		public static void IsNotEmpty(Guid arg, string argumentName)
		{
			if (Guid.Empty.Equals(arg))
			{
				throw new ValidationException("GUID cannot be empty.", argumentName);
			}
		}

		public static void IsNotNullOrWhiteSpace(string arg, string argumentName)
		{
			if (String.IsNullOrWhiteSpace(arg))
			{
				throw new ValidationException("Can't be null or whitespace.", argumentName);
			}
		}

		public static void IsPositive(long arg, string argumentName)
		{
			if (arg < 1)
			{
				throw new ValidationException("Must be a positive number.", argumentName);
			}
		}

		public static void IsDate(DateTime arg, string argumentName)
		{
			if (arg.TimeOfDay != TimeSpan.Zero)
			{
				throw new ValidationException("Date cannot has time of day.", argumentName);
			}
		}

		public static void IsNotDefault(DateTime dateTime, string argumentName)
		{
			if (dateTime == default)
			{
				throw new ValidationException("Can't be default date.", argumentName);
			}
		}
	}
}