using Amazon.S3;
using Amazon.S3.Model;
using SeiyuuMoe.Domain.S3;
using SeiyuuMoe.Infrastructure.Configuration;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.Infrastructure.S3
{
	public class S3Service: IS3Service
	{
		const string BgJobsStateFileName = "state.json";

		public async Task<MalBgJobsState> GetBgJobsStateAsync(string configBucket)
        {
			using var amazonS3Client = new AmazonS3Client();
			var getObjectRequest = new GetObjectRequest
			{
				BucketName = configBucket,
				Key = BgJobsStateFileName
			};

			var response = await amazonS3Client.GetObjectAsync(getObjectRequest);
			using var streamReader = new StreamReader(response.ResponseStream);
			var responseBody = await streamReader.ReadToEndAsync();

			return JsonSerializer.Deserialize<MalBgJobsState>(responseBody);
		}

		public async Task PutBgJobsStateAsync(string configBucket, MalBgJobsState malBgJobsState)
		{
			using var amazonS3Client = new AmazonS3Client();
			var body = JsonSerializer.Serialize(malBgJobsState);
			var request = new PutObjectRequest
			{
				BucketName = configBucket,
				Key = BgJobsStateFileName,
				ContentBody = body
			};

			await amazonS3Client.PutObjectAsync(request);
		}
	}
}