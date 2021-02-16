using SeiyuuMoe.Domain.Entities;
using SeiyuuMoe.Domain.Repositories;
using SeiyuuMoe.Infrastructure.Warehouse.Repositories.Interfaces;
using SeiyuuMoe.Infrastructure.Warehouse.VndbEntities;
using SeiyuuMoe.VndbBackgroundJobs.Application.Helpers;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Application.Handlers
{
	public class AddOrUpdateVisualNovelsHandler: IVndbJobHandler
	{
		private const int BatchSize = 100;

		private readonly IVisualNovelRepository _visualNovelRepository;
		private readonly IVndbVisualNovelRepository _vndbVisualNovelRepository;

		public AddOrUpdateVisualNovelsHandler(IVisualNovelRepository visualNovelRepository, IVndbVisualNovelRepository vndbVisualNovelRepository)
		{
			_visualNovelRepository = visualNovelRepository;
			_vndbVisualNovelRepository = vndbVisualNovelRepository;
		}

		public async Task HandleAsync()
		{
			Console.WriteLine("Started AddOrUpdateVisualNovelsHandler job");

			var totalVndbVisualNovels = await _vndbVisualNovelRepository.GetCountAsync();

			var batchCount = totalVndbVisualNovels / BatchSize + 1;

			for (int i = 0; i < batchCount; i++)
			{
				var vndbVisualNovels = await _vndbVisualNovelRepository.GetOrderedPageByAsync(i, BatchSize);

				foreach (var vndbVisualNovel in vndbVisualNovels.Results)
				{
					await InsertOrUpdateVisualNovelAsync(vndbVisualNovel);
				}
			}

			Console.WriteLine("Finished AddOrUpdateVisualNovelsHandler job");
		}

		private async Task InsertOrUpdateVisualNovelAsync(VndbVisualNovel vndbVisualNovel)
		{
			var existingVisualNovel = await _visualNovelRepository.GetAsync(vndbVisualNovel.Id);
			
			if (existingVisualNovel is null)
			{
				await InsertVisualNovelAsync(vndbVisualNovel);
			}
			else
			{
				await UpdateVisualNovelAsync(existingVisualNovel, vndbVisualNovel);
			}
		}

		private async Task UpdateVisualNovelAsync(VisualNovel existingVisualNovel, VndbVisualNovel vndbVisualNovel)
		{
			existingVisualNovel.VndbId = vndbVisualNovel.Id;
			existingVisualNovel.Title = vndbVisualNovel.Title;
			existingVisualNovel.TitleOriginal = vndbVisualNovel.TitleOriginal;
			existingVisualNovel.Alias = vndbVisualNovel.Alias;
			existingVisualNovel.About = vndbVisualNovel.Description;
			existingVisualNovel.Popularity = vndbVisualNovel.VoteCount;
			existingVisualNovel.ImageUrl = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(vndbVisualNovel.Image);

			await _visualNovelRepository.UpdateAsync(existingVisualNovel);
		}

		private Task InsertVisualNovelAsync(VndbVisualNovel vndbVisualNovel) => _visualNovelRepository.AddAsync(
			new VisualNovel
			{
				Id = Guid.NewGuid(),
				VndbId = vndbVisualNovel.Id,
				Title = vndbVisualNovel.Title,
				TitleOriginal = vndbVisualNovel.TitleOriginal,
				Alias = vndbVisualNovel.Alias,
				About = vndbVisualNovel.Description,
				Popularity = vndbVisualNovel.VoteCount,
				ImageUrl = VndbParserHelper.GenerateVndbVisualNovelImageUrlFromImageId(vndbVisualNovel.Image)
			}
		);
	}
}
