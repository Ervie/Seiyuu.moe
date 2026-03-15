using System;

namespace SeiyuuMoe.Domain.ScheduleItems
{
	public record AnimeScheduleItem(Guid Id, long MalId, DateTime ModificationDate);
}
