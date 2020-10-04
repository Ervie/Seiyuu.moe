using SeiyuuMoe.Domain.SqsMessages;
using System;
using System.Threading.Tasks;

namespace SeiyuuMoe.Domain.Publishers
{
	public interface IAnimeUpdatePublisher
	{
		Task PublishAnimeUpdateAsync(UpdateAnimeMessage updateAnimeMessage);
	}
}