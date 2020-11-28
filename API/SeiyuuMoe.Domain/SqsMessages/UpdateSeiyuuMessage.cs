using System;

namespace SeiyuuMoe.Domain.SqsMessages
{
	public class UpdateSeiyuuMessage
	{
		public Guid Id { get; set; }

		public long MalId { get; set; }
	}
}