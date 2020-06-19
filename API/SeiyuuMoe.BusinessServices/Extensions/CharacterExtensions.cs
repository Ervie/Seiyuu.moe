using SeiyuuMoe.Contracts.Dtos;
using SeiyuuMoe.Data.Model;

namespace SeiyuuMoe.BusinessServices.Extensions
{
	public static class CharacterExtensions
	{
		private const string malCharacterBaseUrl = "https://myanimelist.net/character/";

		public static CharacterTableEntryDto ToCharacterTableEntryDto(this Character character)
			=> new CharacterTableEntryDto(
				character.MalId,
				character.Name,
				character.ImageUrl,
				malCharacterBaseUrl + character.MalId
			);
	}
}