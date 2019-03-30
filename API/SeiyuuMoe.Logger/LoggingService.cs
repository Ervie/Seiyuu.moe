using NLog;
using System;
using System.IO;

namespace SeiyuuMoe.Logger
{
	public class LoggingService: ILoggingService
	{
		private NLog.Logger logger;

		private readonly string logDirectory;

		public LoggingService()
		{
			logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

			if (!Directory.Exists(logDirectory))
				Directory.CreateDirectory(logDirectory);

			logger = LogManager.GetCurrentClassLogger();
		}

		public void Info(string message)
		{
			logger.Log(LogLevel.Info, message);
		}

		public void Error(string message)
		{
			logger.Log(LogLevel.Error, message);
		}

		public void Log(string message)
		{
			logger.Log(LogLevel.Info, message);
		}
	}
}
