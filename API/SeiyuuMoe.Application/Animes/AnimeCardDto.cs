using System;

namespace SeiyuuMoe.Application.Animes
{
	public class AnimeCardDto
	{
		public string Title { get; }

		public string ImageUrl { get; }

		public long MalId { get; }

		public string JapaneseTitle { get; }

		public string TitleSynonyms { get; }

		public string About { get; }

		public DateTime? AiringDate { get; }

		public string Status { get; }

		public string Type { get; }

		public string Season { get; }

		public AnimeCardDto(
			string title,
			string imageUrl,
			long malId,
			string japaneseTitle,
			string titleSynonyms,
			string about,
			DateTime? airingDate,
			string status,
			string type,
			string season
		)
		{
			Title = title;
			ImageUrl = imageUrl;
			MalId = malId;
			JapaneseTitle = japaneseTitle;
			TitleSynonyms = titleSynonyms;
			About = about;
			AiringDate = airingDate;
			Status = status;
			Type = type;
			Season = season;
		}
	}
}