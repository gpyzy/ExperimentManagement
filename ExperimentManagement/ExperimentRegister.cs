using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Threading;

namespace ExperimentManagement
{
	public class ExperimentRegister
	{

		private bool stopWatchStats = false;
		private bool callStackStats = false;
		private ILog logger = new Logger();
		private IExperimentManager expMgr = new ExperimentManager();

		private Dictionary<string, ExperimentMetadata> experimentList = new Dictionary<string, ExperimentMetadata>();
		private ConcurrentDictionary<int, SortedList<string, int>> experimentStatistics = new ConcurrentDictionary<int, Dictionary<string, int>>();

		private ExperimentRegister()
		{
			//System.Threading.Thread
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
		public static ExperimentRegister Register(string experimentId, string experimentName = "")
		{
			Instance.experimentList.Add(experimentId, ExperimentMetadata.New(experimentId, experimentName));
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

		public static ExperimentRegister WithCallStackStats(bool callStackStats)
		{
			Instance.callStackStats = callStackStats;
			return Instance;
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

				var actionToRun = experimentAction;

				if (Instance.stopWatchStats)
				{
					actionToRun = (char pr) =>
					{
						/// TODO - use the elapsedTime for performance statistics
						var elapsedTime = ExperimentWithStopWatch(actionToRun, pr);
					};
				}
				if (Instance.callStackStats)
				{
					actionToRun = (char pr) =>
					{
						ExperimentWithCallStackStats(actionToRun, pr, experimentId);

					};
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


		private static void ExperimentWithCallStackStats(Action<char> experimentAction, char input, string experimentId)
		{
			experimentAction(input);

			try
			{
				var threadId = Thread.CurrentThread.ManagedThreadId;
				var sortList = PrepareExperimentStatistics(threadId);

				if (sortList.ContainsKey(experimentId))
				{
					sortList[experimentId]++;
				}
				else
				{
					sortList.Add(experimentId, 1);
				}
			}
			catch (Exception ex)
			{
				Instance.logger.Error("ExperimentWithCallStackStats faliure: " + ex.Message);
			}



		}

		private static SortedList<string, int> PrepareExperimentStatistics(int key)
		{
			if (!Instance.experimentStatistics.ContainsKey(key))
			{
				if (!Instance.experimentStatistics.TryAdd(key, new SortedList<string, int>()))
				{
					/// https://stackoverflow.com/questions/11501931/can-concurrentdictionary-tryadd-fail
					/// Nothing to do here
				}
			}

			return Instance.experimentStatistics[key];
		}


		#endregion


		private static class InternalClass
		{
			public static ExperimentRegister instance = new ExperimentRegister();
		}
	}
}
