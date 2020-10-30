using JikanDotNet;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.Jikan
{
	public class JikanService : IMalApiService
	{
		private readonly IJikan _jikanClient;

		public JikanService(IJikan jikanClient)
		{
			_jikanClient = jikanClient;
		}

		public async Task<MalAnimeUpdateData> GetAnimeDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetAnime(malId);

			if (parsedData is null)
			{
				return null;
			}

			return new MalAnimeUpdateData(
				parsedData.Title,
				parsedData.Synopsis,
				parsedData.TitleEnglish,
				parsedData.TitleJapanese,
				parsedData.TitleSynonyms.Any() ? string.Join(';', parsedData.TitleSynonyms) : string.Empty,
				parsedData.Members,
				EmptyStringIfPlaceholder(parsedData.ImageURL),
				parsedData.Aired?.From,
				parsedData.Type,
				parsedData.Status,
				parsedData.Premiered
			);
		}

		private string EmptyStringIfPlaceholder(string imageUrl)
		{
			var isEmptyOrPlaceholder = string.IsNullOrEmpty(imageUrl) ||
				imageUrl.Equals("https://cdn.myanimelist.net/images/questionmark_23.gif") ||
				imageUrl.Equals("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png");

			return isEmptyOrPlaceholder ? string.Empty : imageUrl;
		}
	}
}