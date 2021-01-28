using SeiyuuMoe.Domain.Entities;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Repositories
{
	public interface IVisualNovelRepository
	{
		Task<VisualNovel> GetAsync(long vndbId);

		Task AddAsync(VisualNovel visualNovel);

		Task UpdateAsync(VisualNovel visualNovel);
	}
}