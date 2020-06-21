using SeiyuuMoe.Domain.ComparisonEntities;

namespace SeiyuuMoe.Application.Character.Extensions
{
	public static class CharacterExtensions
	{
		private const string malCharacterBaseUrl = "https://myanimelist.net/character/";

		public static CharacterTableEntry ToCharacterTableEntry(this Domain.Entities.Character character)
			=> new CharacterTableEntry(
				character.MalId,
				character.Name,
				character.ImageUrl,
				malCharacterBaseUrl + character.MalId
			);
	}
}