using SeiyuuMoe.Domain.SqsMessages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Publishers
{
	public interface ICharactersUpdatePublisher
	{
		Task PublishCharacterUpdatesAsync(IReadOnlyList<UpdateCharacterMessage> messages);
	}
}