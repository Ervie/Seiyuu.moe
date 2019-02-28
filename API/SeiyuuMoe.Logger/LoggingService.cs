using NLog;
using System;
using System.IO;

namespace SeiyuuMoe.Logger
{
	public class LoggingService
	{
		private NLog.Logger logger;

		private readonly string logDirectory;

		public LoggingService()
		{
			logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

			if (!Directory.Exists(logDirectory))
				Directory.CreateDirectory(logDirectory);

			logger = LogManager.GetCurrentClassLogger();

			logger.Info("Logger init");
		}

		public void Log(string message)
		{
			logger.Log(LogLevel.Info, message);
		}
	}
}
