using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.VndbBackgroundJobs.Application.Helpers;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Application.Handlers
{
	public class AddOrUpdateVisualNovelCharactersHandler : IVndbJobHandler
	{
		private const int BatchSize = 100;

		private readonly IVisualNovelCharacterRepository _visualNovelCharacterRepository;
		private readonly IVndbCharacterRepository _vndbCharacterRepository;

		public AddOrUpdateVisualNovelCharactersHandler(
			IVisualNovelCharacterRepository visualNovelCharacterRepository,
			IVndbCharacterRepository vndbCharacterRepository
		)
		{
			_visualNovelCharacterRepository = visualNovelCharacterRepository;
			_vndbCharacterRepository = vndbCharacterRepository;
		}

		public async Task HandleAsync()
		{
			Console.WriteLine("Started AddOrUpdateVisualNovelCharactersHandler job");

			var totalVndbCharacters = await _vndbCharacterRepository.GetCountAsync();

			var batchCount = totalVndbCharacters / BatchSize + 1;

			for (int i = 0; i < batchCount; i++)
			{
				var vndbCharacters = await _vndbCharacterRepository.GetOrderedPageByAsync(i, BatchSize);

				foreach (var vndbCharacter in vndbCharacters.Results)
				{
					await InsertOrUpdateCharacterAsync(vndbCharacter);
				}
			}

			Console.WriteLine("Finished AddOrUpdateVisualNovelCharactersHandler job");
		}

		private async Task InsertOrUpdateCharacterAsync(VndbCharacter vndbCharacter)
		{
			var existingCharacter = await _visualNovelCharacterRepository.GetAsync(vndbCharacter.Id);

			if (existingCharacter is null)
			{
				await InsertCharacterAsync(vndbCharacter);
			}
			else
			{
				await UpdateCharacterAsync(existingCharacter, vndbCharacter);
			}
		}

		private async Task UpdateCharacterAsync(VisualNovelCharacter existingCharacter, VndbCharacter vndbCharacter)
		{
			existingCharacter.VndbId = vndbCharacter.Id;
			existingCharacter.Name = vndbCharacter.Name;
			existingCharacter.KanjiName = vndbCharacter.NameOriginal;
			existingCharacter.Nicknames = vndbCharacter.Alias;
			existingCharacter.About = vndbCharacter.Description;
			existingCharacter.ImageUrl = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(vndbCharacter.Image);

			await _visualNovelCharacterRepository.UpdateAsync(existingCharacter);
		}

		private Task InsertCharacterAsync(VndbCharacter vndbCharacter) => _visualNovelCharacterRepository.AddAsync(
			new VisualNovelCharacter
			{
				Id = Guid.NewGuid(),
				VndbId = vndbCharacter.Id,
				Name = vndbCharacter.Name,
				KanjiName = vndbCharacter.NameOriginal,
				Nicknames = vndbCharacter.Alias,
				About = vndbCharacter.Description,
				ImageUrl = VndbParserHelper.GenerateVndbCharacterImageUrlFromImageId(vndbCharacter.Image)
			}
		);
	}
}