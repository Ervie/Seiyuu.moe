using System;

namespace SeiyuuMoe.Domain.SqsMessages
{
	public class UpdateCharacterMessage
	{
		public Guid Id { get; set; }

		public long MalId { get; set; }
	}
}