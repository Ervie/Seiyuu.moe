using System.Threading.Tasks;

namespace SeiyuuMoe.VndbBackgroundJobs.Application.Handlers
{
	public interface IVndbJobHandler
	{
		Task HandleAsync();
	}
}