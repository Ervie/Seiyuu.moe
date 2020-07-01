using SeiyuuMoe.Domain.Entities;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class BlacklistedIdBuilder
	{
		private long _malId;
		private long _id;
		private string _reason;
		private string _entityType;

		public BlacklistedId Build()
			=> new BlacklistedId
			{
				EntityType = _entityType,
				Reason = _reason,
				MalId = _malId,
				Id = _id
			};

		public BlacklistedIdBuilder WithMalId(long malId)
		{
			_malId = malId;
			return this;
		}

		public BlacklistedIdBuilder WithId(long id)
		{
			_id = id;
			return this;
		}

		public BlacklistedIdBuilder WithEntityType(string entityType)
		{
			_entityType = entityType;
			return this;
		}

		public BlacklistedIdBuilder WithReason(string reason)
		{
			_reason = reason;
			return this;
		}

		public BlacklistedIdBuilder WithId(string reason)
		{
			_reason = reason;
			return this;
		}
	}
}