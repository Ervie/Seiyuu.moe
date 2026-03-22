using JikanDotNet;
using System.Collections.Generic;

namespace SeiyuuMoe.Tests.Common.Builders.Jikan
{
	public static class AnimeTitlesBuilder
	{
		public static List<TitleEntry> Build(
			string defaultTitle = null,
			string englishTitle = null,
			string japaneseTitle = null,
			ICollection<string> titleSynonyms = null
		)
		{
			var titles = new List<TitleEntry>();

			if (!string.IsNullOrWhiteSpace(defaultTitle))
			{
				titles.Add(new TitleEntry { Type = "Default", Title = defaultTitle });
			}

			if (!string.IsNullOrWhiteSpace(englishTitle))
			{
				titles.Add(new TitleEntry { Type = "English", Title = englishTitle });
			}

			if (!string.IsNullOrWhiteSpace(japaneseTitle))
			{
				titles.Add(new TitleEntry { Type = "Japanese", Title = japaneseTitle });
			}

			if (titleSynonyms != null)
			{
				foreach (var titleSynonym in titleSynonyms)
				{
					if (!string.IsNullOrWhiteSpace(titleSynonym))
					{
						titles.Add(new TitleEntry { Type = "Synonym", Title = titleSynonym });
					}
				}
			}

			return titles;
		}
	}
}
