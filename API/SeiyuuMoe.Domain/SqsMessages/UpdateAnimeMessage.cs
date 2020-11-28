using System;

namespace SeiyuuMoe.Domain.SqsMessages
{
	public  class UpdateAnimeMessage
	{
		public Guid Id { get; set; }

		public long MalId { get; set; }
	}
}