using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.MalUpdateData;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Domain.Services;
using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.Application.Handlers
{
	public class UpdateCharacterHandler
	{
		private readonly ICharacterRepository _characterRepository;
		private readonly IMalApiService _malApiService;

		public UpdateCharacterHandler(ICharacterRepository characterRepository, IMalApiService malApiService)
		{
			_characterRepository = characterRepository;
			_malApiService = malApiService;
		}

		public async Task HandleAsync(UpdateCharacterMessage updateCharacterMessage)
		{
			var characterToUpdate = await _characterRepository.GetAsync(updateCharacterMessage.MalId);

			if (characterToUpdate == null)
			{
				return;
			}

			var updateData = await _malApiService.GetCharacterDataAsync(updateCharacterMessage.MalId);

			if (updateData == null)
			{
				return;
			}

			UpdateCharacter(characterToUpdate, updateData);
			await _characterRepository.UpdateAsync(characterToUpdate);
		}

		private void UpdateCharacter(AnimeCharacter characterToUpdate, MalCharacterUpdateData updateData)
		{
			characterToUpdate.Name = updateData.Name;
			characterToUpdate.About = updateData.About;
			characterToUpdate.KanjiName = updateData.JapaneseName;
			characterToUpdate.ImageUrl = updateData.ImageUrl;
			characterToUpdate.Nicknames = !string.IsNullOrWhiteSpace(updateData.Nicknames) ? updateData.Nicknames : characterToUpdate.Nicknames;
			characterToUpdate.Popularity = updateData.Popularity;
		}
	}
}