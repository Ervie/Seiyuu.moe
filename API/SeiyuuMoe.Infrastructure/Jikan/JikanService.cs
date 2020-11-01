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
				(parsedData.TitleSynonyms != null && parsedData.TitleSynonyms.Any()) ? string.Join(';', parsedData.TitleSynonyms) : string.Empty,
				parsedData.Members,
				EmptyStringIfPlaceholder(parsedData.ImageURL),
				parsedData.Aired?.From,
				parsedData.Type,
				parsedData.Status,
				parsedData.Premiered
			);
		}

		public async Task<MalCharacterUpdateData> GetCharacterDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetCharacter(malId);

			if (parsedData is null)
			{
				return null;
			}

			return new MalCharacterUpdateData(
				parsedData.Name,
				parsedData.About,
				parsedData.NameKanji,
				EmptyStringIfPlaceholder(parsedData.ImageURL),
				(parsedData.Nicknames != null && parsedData.Nicknames.Any()) ? string.Join(';', parsedData.Nicknames) : string.Empty,
				parsedData.MemberFavorites
			);
		}

		public async Task<MalSeasonUpdateData> GetSeasonDataAsync()
		{
			var parsedData = await _jikanClient.GetSeasonArchive();

			if (parsedData is null)
			{
				return null;
			}

			var latestYear = parsedData?.Archives?.FirstOrDefault();

			return new MalSeasonUpdateData(latestYear.Year, latestYear.Season.Last().ToString());
		}

		private string EmptyStringIfPlaceholder(string imageUrl)
		{
			var isEmptyOrPlaceholder = string.IsNullOrWhiteSpace(imageUrl) ||
				imageUrl.Equals("https://cdn.myanimelist.net/images/questionmark_23.gif") ||
				imageUrl.Equals("https://cdn.myanimelist.net/img/sp/icon/apple-touch-icon-256.png");

			return isEmptyOrPlaceholder ? string.Empty : imageUrl;
		}
	}
}