namespace SeiyuuMoe.Common.Extensions
{
	public static class StringExtensions
	{
		public static string SwapNameSurname(this string inputString)
		{
			var parts = inputString.Split(' ');

			if (parts.Length < 2)
			{
				return inputString;
			}
			else
			{
				return parts[1] + ' ' + parts[0];
			}
		}
	}
}