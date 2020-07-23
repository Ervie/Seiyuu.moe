using NLog;
using System;
using System.IO;

namespace SeiyuuMoe.Infrastructure.Logger
{
	public class LoggingService: ILoggingService
	{
		private readonly NLog.Logger _logger;

		private readonly string _logDirectory;

		public LoggingService()
		{
			_logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

			if (!Directory.Exists(_logDirectory))
				Directory.CreateDirectory(_logDirectory);

			_logger = LogManager.GetCurrentClassLogger();
		}

		public void Info(string message)
		{
			_logger.Log(LogLevel.Info, message);
		}

		public void Error(string message)
		{
			_logger.Log(LogLevel.Error, message);
		}

		public void Log(string message)
		{
			_logger.Log(LogLevel.Info, message);
		}
	}
}
