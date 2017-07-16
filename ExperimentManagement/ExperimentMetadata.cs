using System;
namespace ExperimentManagement
{
	public class ExperimentMetadata
	{
		private ExperimentMetadata()
		{
		}

		public string Id { get; private set; }
		public string Name { get; private set; }

		public static ExperimentMetadata New(string experimentId, string experimentName)
		{
			return new ExperimentMetadata { Name = experimentName, Id = experimentId };
		}
	}
}