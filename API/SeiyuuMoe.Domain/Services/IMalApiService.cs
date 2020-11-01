using SeiyuuMoe.Domain.MalUpdateData;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Services
{
	public interface IMalApiService
	{
		Task<MalAnimeUpdateData> GetAnimeDataAsync(long malId);

		Task<MalSeasonUpdateData> GetSeasonDataAsync();

		Task<MalCharacterUpdateData> GetCharacterDataAsync(long malId);
	}
}