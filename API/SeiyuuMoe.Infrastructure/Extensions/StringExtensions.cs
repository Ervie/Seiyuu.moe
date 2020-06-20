namespace SeiyuuMoe.Infrastructure.Extensions
{
	public static class StringExtensions
	{
		public static string SwapWords(this string inputString, char delimiter = ' ')
		{
			var parts = inputString.Split(delimiter);

			if (parts.Length < 2)
			{
				return inputString;
			}
			else
			{
				return parts[1] + delimiter + parts[0];
			}
		}
	}
}