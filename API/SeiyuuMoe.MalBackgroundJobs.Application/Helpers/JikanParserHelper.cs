using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Helpers
{
	internal static class JikanParserHelper
	{
		public static AnimeStatusId? GetUpdatedAnimeStatus(AnimeStatusId? currentStatus, string newStatus) =>
			Enum.TryParse(newStatus?.Replace(" ", ""), out AnimeStatusId parsedAnimeStatus) ?
				parsedAnimeStatus :
				currentStatus;

		public static AnimeTypeId? GetUpdatedAnimeType(AnimeTypeId? currentType, string newType) =>
			Enum.TryParse(newType, out AnimeTypeId parsedAnimeType) ?
				parsedAnimeType :
				currentType;

		public static (string, int)? GetSeasonPartsByName(string season)
		{
			var seasonParts = season.Split(' ');

			if (seasonParts.Length < 2)
				return null;

			var isYearNumber = int.TryParse(seasonParts[1], out int year);
			var seasonName = seasonParts[0];

			if (!isYearNumber)
				return null;

			return (seasonName, year);
		}

		public static (string, int)? GetSeasonPartsByAiringDate(DateTime? airingDate)
		{
			if (!airingDate.HasValue)
				return null;

			var airingDay = airingDate.Value.DayOfYear;
			var airingYear = airingDate.Value.Year;

			var seasonName = airingDay switch
			{
				var day when day > 349 || day < 75 => "Winter",
				var day when day >= 75 && day < 166 => "Spring",
				var day when day >= 167 && day < 258 => "Summer",
				_ => "Fall"
			};

			if (airingDay > 349)
			{
				airingYear++;
			}

			return (seasonName, airingYear);
		}
	}
}