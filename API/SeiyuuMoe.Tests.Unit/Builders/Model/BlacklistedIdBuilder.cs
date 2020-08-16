using SeiyuuMoe.Domain.Entities;
using System;

namespace SeiyuuMoe.Tests.Unit.Builders.Model
{
	public class BlacklistedIdBuilder
	{
		private long _malId;
		private Guid _id;
		private string _reason;
		private string _entityType;

		public Blacklist Build()
			=> new Blacklist
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

		public BlacklistedIdBuilder WithId(Guid id)
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