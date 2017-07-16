using System;
namespace ExperimentManagement
{
	public class ExperimentMetadata
	{
		private ExperimentMetadata()
		{
		}

		public string Name { get; private set; }

		public static ExperimentMetadata New(string experimentId)
		{
			return new ExperimentMetadata { Name = experimentId };
		}

	}
}