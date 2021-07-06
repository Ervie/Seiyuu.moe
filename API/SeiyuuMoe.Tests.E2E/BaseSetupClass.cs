using RestEase;
using RestEase.Implementation;
using System;
using System.Net.Http;

namespace SeiyuuMoe.Tests.E2E
{
	public abstract class BaseSetupClass
	{
		private const string BaseUrl = "https://seiyuu.moe:9000/api";

		protected IRequester requester;

		public BaseSetupClass()
		{
			var httpClient = new HttpClient()
			{
				BaseAddress = new Uri(BaseUrl),
				Timeout = new TimeSpan(0, 10, 0)
			};

			requester = new Requester(httpClient);
		}
	}
}