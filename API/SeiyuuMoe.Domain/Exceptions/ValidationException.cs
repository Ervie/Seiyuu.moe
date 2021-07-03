using System;

namespace SeiyuuMoe.Domain.Exceptions
{
	public class ValidationException : Exception
	{
		public string ArgumentName { get; }

		public ValidationException(string message, string argumentName) : base(message)
		{
			ArgumentName = argumentName;
		}
	}
}