using SeiyuuMoe.Infrastructure.Configuration;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.S3
{
	public interface IS3Service
	{
		Task<MalBgJobsState> GetBgJobsStateAsync(string configBucket);

		Task PutBgJobsStateAsync(string configBucket, MalBgJobsState malBgJobsState);
	}
}