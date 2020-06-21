using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Domain.ComparisonEntities;
using System;

namespace SeiyuuMoe.Application.Seiyuu.Extensions
{
	public static class SeiyuuExtensions
	{
		private const string malPersonBaseUrl = "https://myanimelist.net/people/";

		public static SeiyuuSearchEntryDto ToSeiyuuSearchEntryDto(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuSearchEntryDto (seiyuu.MalId, seiyuu.Name, seiyuu.ImageUrl);

		public static SeiyuuCardDto ToSeiyuuCardDto(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuCardDto(
				seiyuu.MalId,
				seiyuu.Name,
				seiyuu.ImageUrl,
				seiyuu.JapaneseName,
				!string.IsNullOrWhiteSpace(seiyuu.Birthday)
					? DateTime.ParseExact(seiyuu.Birthday, "dd-MM-yyyy", null)
					: (DateTime?)null,
				seiyuu.About
			);

		public static SeiyuuTableEntry ToSeiyuuTableEntry(this Domain.Entities.Seiyuu seiyuu)
			=> new SeiyuuTableEntry(
				seiyuu.MalId,
				seiyuu.Name,
				seiyuu.ImageUrl,
				malPersonBaseUrl + seiyuu.MalId
			);
	}
}