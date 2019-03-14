using System;
using System.Collections.Generic;
using System.Text;

namespace SeiyuuMoe.Logger
{
	public interface ILoggingService
	{
		void Info(string message);

		void Error(string message);

		void Log(string message);
	}
}
