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
        private ConcurrentDictionary<int, SortedList<string, int>> experimentStatistics = new ConcurrentDictionary<int, SortedList<string, int>>();

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
                Action actionToRun = () => { experimentAction(peekResult); };
                if (Instance.stopWatchStats)
                {
                    actionToRun = ExperimentWithStopWatchStats(actionToRun);
                }

                if (Instance.callStackStats)
                {
                    actionToRun = ExperimentWithCallStackStats(actionToRun, experimentId);
                }

                actionToRun();
            }
            else
            {
                Instance.logger.Warn("The experiment '{0}' is not regiestered", experimentId);
            }

            
        }

        private static Action ExperimentWithStopWatchStats(Action experimentAction)
        {
            return () =>
            {
                
                Stopwatch sw = new Stopwatch();
                sw.Start();
                experimentAction();
                sw.Stop();
                //return sw.ElapsedMilliseconds;
                Instance.logger.Debug("ExperimentWithStopWatchStats called.");
            };
        }


        private static Action ExperimentWithCallStackStats(Action experimentAction, string experimentId)
        {
            return () =>
            {
                experimentAction();

                /// TODO - performance enhancements
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

                Instance.logger.Debug("ExperimentWithCallStackStats called.");
            };

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
