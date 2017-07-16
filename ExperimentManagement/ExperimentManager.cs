using System;

namespace ExperimentManagement
{
	public class ExperimentManager : IExperimentManager
	{
		public bool Determine(string experimentName)
		{
			throw new NotImplementedException();
		}

		public char Peek(string experimentId)
		{
			var now = DateTime.Now.Millisecond;
			var result = now % 3;

			return (char)(65 + result);
		}
	}



}
