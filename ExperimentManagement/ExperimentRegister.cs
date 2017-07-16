using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ExperimentManagement
{
	public class ExperimentRegister
	{

		private bool stopWatchStats = false;
		private ILog logger = new Logger();
		private IExperimentManager expMgr = new ExperimentManager();

		private Dictionary<string, ExperimentMetadata> experimentList = new Dictionary<string, ExperimentMetadata>();


		private ExperimentRegister()
		{

		}

		#region properties
		private static ExperimentRegister Instance
		{
			get
			{
				return InternalClass.instance;
			}
		}

		#endregion

		#region Methods
		public static ExperimentRegister Register(string experimentName)
		{
			Instance.experimentList.Add(experimentName, ExperimentMetadata.New(experimentName));
			return Instance;
		}
		public static ExperimentRegister Register(IList<string> experimentNames)
		{
			foreach (var name in experimentNames)
			{
				Register(name);
			}

			return Instance;
		}

		public static ExperimentRegister WithStopWatchStats(bool stopWatchStats)
		{
			Instance.stopWatchStats = stopWatchStats;
			return Instance;
		}

		public static ExperimentRegister WithCallStackPath(bool callstackPath)
		{
			throw new NotImplementedException();
		}

		public static ExperimentRegister WithLog(ILog logger)
		{
			Instance.logger = logger;
			return Instance;
		}

		public static void Experiment(string experimentId, Action<char> experimentAction)
		{
			if (Instance.experimentList.ContainsKey(experimentId))
			{
				var peekResult = Instance.expMgr.Peek(experimentId);

				if (Instance.stopWatchStats)
				{
					/// TODO - use the elapsedTime for performance statistics
					var elapsedTime = ExperimentWithStopWatch(experimentAction, peekResult);
				}
				else
				{
					experimentAction(peekResult);
				}
			}
			else
			{
				Instance.logger.Warn("The experiment '{0}' is not regiestered", experimentId);
			}

		}

		private static long ExperimentWithStopWatch(Action<char> experimentAction, char input)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			experimentAction(input);
			sw.Stop();
			return sw.ElapsedMilliseconds;
		}
		#endregion


		private static class InternalClass
		{
			public static ExperimentRegister instance = new ExperimentRegister();
		}
	}
}
