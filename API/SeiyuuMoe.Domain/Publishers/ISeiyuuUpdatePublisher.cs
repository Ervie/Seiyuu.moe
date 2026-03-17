using SeiyuuMoe.Domain.SqsMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Publishers
{
	public interface ISeiyuuUpdatePublisher
	{
		Task PublishSeiyuuUpdatesAsync(IReadOnlyList<UpdateSeiyuuMessage> messages);
	}
}