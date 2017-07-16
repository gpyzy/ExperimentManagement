using System;
namespace ExperimentManagement
{
	public interface IExperimentManager
	{
		char Peek(string experimentId);
		bool Determine(string experimentId);
	}
}
