using SeiyuuMoe.Domain.SqsMessages;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Publishers
{
	public interface ISeiyuuUpdatePublisher
	{
		Task PublishSeiyuuUpdateAsync(UpdateSeiyuuMessage updateAnimeMessage, int delayInSeconds = 0);
	}
}