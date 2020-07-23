using System.Collections.Generic;

namespace SeiyuuMoe.Domain.WebEssentials
{
	public class QueryResponse<TEntity>
		where TEntity : class
	{
		public QueryResponse(bool found)
		{
			Found = found;
			Payload = null;
			ValidationErrors = new Dictionary<string, string>();
			Error = string.Empty;
		}

		public QueryResponse(
			TEntity payload = null,
			IReadOnlyDictionary<string, string> validationErrors = null,
			string errorKey = null)
		{
			Payload = payload;
			ValidationErrors = validationErrors ?? new Dictionary<string, string>();
			Error = errorKey ?? string.Empty;
			Found = payload != null;
		}

		public bool Found { get; }

		public TEntity Payload { get; }

		public IReadOnlyDictionary<string, string> ValidationErrors { get; }

		public string Error { get; set; }
	}
}