using SeiyuuMoe.Domain.ComparisonEntities;
using System;

namespace SeiyuuMoe.Application.Seiyuus.Extensions
{
	public static class SeiyuuExtensions
	{
		private const string MalPersonBaseUrl = "https://myanimelist.net/people/";

		public static SeiyuuSearchEntryDto ToSeiyuuSearchEntryDto(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuSearchEntryDto(seiyuu.MalId, seiyuu.Name, seiyuu.ImageUrl);

		public static SeiyuuCardDto ToSeiyuuCardDto(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuCardDto(
				seiyuu.MalId,
				seiyuu.Name,
				seiyuu.ImageUrl,
				seiyuu.KanjiName,
				seiyuu.Birthday,
				seiyuu.About
			);

		public static SeiyuuTableEntry ToSeiyuuTableEntry(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuTableEntry(
				seiyuu.MalId,
				seiyuu.Name,
				seiyuu.ImageUrl,
				MalPersonBaseUrl + seiyuu.MalId
			);
	}
}