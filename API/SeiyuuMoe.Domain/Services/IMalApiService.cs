using System.Collections.Generic;
using System.Threading.Tasks;
using SeiyuuMoe.Domain.MalUpdateData;

namespace SeiyuuMoe.Domain.Services
{
	public interface IMalApiService
	{
		Task<MalAnimeUpdateData> GetAnimeDataAsync(long malId);

		Task<MalSeasonUpdateData> GetSeasonDataAsync();

		Task<MalCharacterUpdateData> GetCharacterDataAsync(long malId);

		Task<MalSeiyuuUpdateData> GetSeiyuuDataAsync(long malId);
		
		Task<ICollection<MalVoiceActingRoleUpdateData>> GetSeiyuuVoiceActingRolesAsync(long malId);
	}
}