using System;

namespace SeiyuuMoe.Domain.ScheduleItems
{
	public record SeiyuuScheduleItem(Guid Id, long MalId, DateTime ModificationDate);
}
