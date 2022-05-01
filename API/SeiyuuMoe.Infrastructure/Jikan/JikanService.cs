using JikanDotNet;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Services;
using System.Collections.Generic;
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
			var parsedData = await _jikanClient.GetAnimeAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}

			return new MalAnimeUpdateData(
				parsedData.Data.Title,
				parsedData.Data.Synopsis,
				parsedData.Data.TitleEnglish,
				parsedData.Data.TitleJapanese,
				(parsedData.Data.TitleSynonyms != null && parsedData.Data.TitleSynonyms.Any()) ? string.Join(';', parsedData.Data.TitleSynonyms) : string.Empty,
				parsedData.Data.Members,
				EmptyStringIfPlaceholder(parsedData.Data.Images?.JPG?.ImageUrl),
				parsedData.Data.Aired?.From,
				parsedData.Data.Type,
				parsedData.Data.Status,
				parsedData.Data.Season.ToString(),
				parsedData.Data.Year
			);
		}

		public async Task<MalCharacterUpdateData> GetCharacterDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetCharacterAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}

			return new MalCharacterUpdateData(
				parsedData.Data.Name,
				parsedData.Data.About,
				parsedData.Data.NameKanji,
				EmptyStringIfPlaceholder(parsedData.Data.Images?.JPG?.ImageUrl),
				(parsedData.Data.Nicknames != null && parsedData.Data.Nicknames.Any()) ? string.Join(';', parsedData.Data.Nicknames) : string.Empty,
				parsedData.Data.Favorites
			);
		}

		public async Task<MalSeasonUpdateData> GetSeasonDataAsync()
		{
			var parsedData = await _jikanClient.GetSeasonArchiveAsync();

			if (parsedData?.Data is null)
			{
				return null;
			}

			var latestYear = parsedData?.Data?.FirstOrDefault();

			return new MalSeasonUpdateData(latestYear.Year, latestYear.Season.Last().ToString());
		}

		public async  Task<MalSeiyuuUpdateData> GetSeiyuuDataAsync(long malId)
		{
			var parsedData = await _jikanClient.GetPersonAsync(malId);

			if (parsedData?.Data is null)
			{
				return null;
			}
			
			return new MalSeiyuuUpdateData(
				parsedData.Data.Name,
				parsedData.Data.About,
				$"{parsedData.Data.FamilyName ?? string.Empty} {parsedData.Data.GivenName ?? string.Empty}".Trim(),
				EmptyStringIfPlaceholder(parsedData.Data.Images?.JPG?.ImageUrl),
				parsedData.Data.MemberFavorites,
				parsedData.Data.Birthday
			);
		}

		public async Task<ICollection<MalVoiceActingRoleUpdateData>> GetSeiyuuVoiceActingRolesAsync(long malId)
		{
			var parsedData = await _jikanClient.GetPersonVoiceActingRolesAsync(malId);

			if (parsedData?.Data is null)
			{
				return new List<MalVoiceActingRoleUpdateData>();
			}

			return parsedData.Data?.Select(
				x => new MalVoiceActingRoleUpdateData(
					x.Anime.MalId,
					x.Character.MalId,
					x.Role
				)
			).ToList() ?? new List<MalVoiceActingRoleUpdateData>();
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