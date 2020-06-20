using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class SeiyuuExtensions
	{
		private const string malPersonBaseUrl = "https://myanimelist.net/people/";

		public static SeiyuuSearchEntryDto ToSeiyuuSearchEntryDto(this Seiyuu seiyuu)
			=> new SeiyuuSearchEntryDto(seiyuu.MalId, seiyuu.Name, seiyuu.ImageUrl);

		public static SeiyuuCardDto ToSeiyuuCardDto(this Seiyuu seiyuu)
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

		public static SeiyuuTableEntryDto ToSeiyuuTableEntryDto(this Seiyuu seiyuu)
			=> new SeiyuuTableEntryDto(
				seiyuu.MalId,
				seiyuu.Name,
				seiyuu.ImageUrl,
				malPersonBaseUrl + seiyuu.MalId
			);
	}
}