using System;
namespace ExperimentManagement
{
	public interface ILog
	{
		void Warn(string message, params string[] values);
		void Error(string message, params string[] values);
		void Debug(string message, params string[] values);
	}
}
