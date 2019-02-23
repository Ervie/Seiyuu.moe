using System.Collections.Generic;

namespace SeiyuuMoe.WebEssentials
{
	public class ApiResult<TEntity> where TEntity : class
	{
		public TEntity Payload { get; set; }

		public Dictionary<string, string> ValidationErrors { get; set; }

		public string Error { get; set; }
	}
}