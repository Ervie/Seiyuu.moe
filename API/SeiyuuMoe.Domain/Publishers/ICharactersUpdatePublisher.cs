using SeiyuuMoe.Domain.SqsMessages;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Publishers
{
	public interface ICharactersUpdatePublisher
	{
		Task PublishCharacterUpdateAsync(UpdateCharacterMessage updateCharacterMessage, int delayInSeconds = 0);
	}
}