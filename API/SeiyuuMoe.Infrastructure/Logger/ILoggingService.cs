namespace SeiyuuMoe.Infrastructure.Logger
{
	public interface ILoggingService
	{
		void Info(string message);

		void Error(string message);

		void Log(string message);
	}
}