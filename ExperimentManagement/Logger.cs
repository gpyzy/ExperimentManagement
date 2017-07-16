using System;
namespace ExperimentManagement
{
	public class Logger : ILog
	{
		public Logger()
		{
		}

		public void Debug(string message, params string[] values)
		{
			Console.WriteLine("Debugger: {0}", string.Format(message, values));
		}

		public void Error(string message, params string[] values)
		{
			Console.WriteLine("Error: {0}", string.Format(message, values));
		}

		public void Warn(string message, params string[] values)
		{
			Console.WriteLine("Warn: {0}", string.Format(message, values));
		}
	}
}
