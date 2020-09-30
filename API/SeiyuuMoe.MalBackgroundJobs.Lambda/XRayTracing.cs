using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.AwsSdk;

namespace SeiyuuMoe.MalBackgroundJobs.Lambda
{
	internal static class XRayTracing
	{
		internal static void Configure()
		{
			AWSSDKHandler.RegisterXRayForAllServices();
#if DEBUG
			AWSXRayRecorder.Instance.ContextMissingStrategy = Amazon.XRay.Recorder.Core.Strategies.ContextMissingStrategy.LOG_ERROR;
#endif
		}
	}
}