using System;

namespace SeiyuuMoe.Domain.MalUpdateData
{
	public class MalAnimeUpdateData
	{
		public string Title { get; private set; }

		public string About { get; private set; }

		public string EnglishTitle { get; private set; }

		public string JapaneseTitle { get; private set; }

		public string TitleSynonyms { get; private set; }

		public int? Popularity { get; private set; }

		public string ImageUrl { get; private set; }

		public DateTime? AiringDate { get; private set; }

		public string Type { get; private set; }

		public string Status { get; private set; }

		public string Season { get; private set; }

		public MalAnimeUpdateData(
			string title,
			string about,
			string englishTitle,
			string japaneseTitle,
			string titleSynonyms,
			int? popularity,
			string imageUrl,
			DateTime? airingDate,
			string type,
			string status,
			string season
		)
		{
			Title = title;
			About = about;
			EnglishTitle = englishTitle;
			JapaneseTitle = japaneseTitle;
			TitleSynonyms = titleSynonyms;
			Popularity = popularity;
			ImageUrl = imageUrl;
			AiringDate = airingDate;
			Type = type;
			Status = status;
			Season = season;
		}
	}
}