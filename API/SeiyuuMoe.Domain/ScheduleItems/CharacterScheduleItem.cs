using System;

namespace SeiyuuMoe.Domain.ScheduleItems
{
	public record CharacterScheduleItem(Guid Id, long MalId, DateTime ModificationDate);
}
