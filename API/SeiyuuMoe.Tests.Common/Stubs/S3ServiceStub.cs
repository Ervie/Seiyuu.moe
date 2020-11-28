using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Infrastructure.Configuration;
using System.Threading.Tasks;

namespace SeiyuuMoe.Tests.Common.Stubs
{
	public class S3ServiceStub : IS3Service
	{
		private MalBgJobsState _state;

		public S3ServiceStub(int lastCheckedId)
		{
			_state = new MalBgJobsState { LastCheckedSeiyuuMalId = lastCheckedId };
		}

		public Task<MalBgJobsState> GetBgJobsStateAsync(string configBucket) => Task.FromResult(_state);

		public Task PutBgJobsStateAsync(string configBucket, MalBgJobsState malBgJobsState)
		{
			_state = malBgJobsState;
			return Task.CompletedTask;
		}
	}
}