using System;

namespace SeiyuuMoe.Common.Extensions
{
	public static class ExceptionExtensions
	{
		public static string GetFullMessage(this Exception ex)
		{
			return ex.InnerException == null
				 ? ex.Message
				 : ex.Message + " --> " + ((ex is AggregateException) ?
					(ex as AggregateException).GetAggregateExceptionsMessage() :
					ex.InnerException.GetFullMessage());
		}

		public static string GetAggregateExceptionsMessage(this AggregateException ex)
		{
			return ex.Flatten().GetFullMessage();
		}
	}
}