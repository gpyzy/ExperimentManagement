using System;
using System.Collections.Generic;
using System.Threading;
using ExperimentManagement;

namespace HelloWorld
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            ExperimentRegister.Register(new List<string> { "Exp01", "Exp02", "Exp03" });
            ExperimentRegister.WithStopWatchStats(true);
            ExperimentRegister.WithCallStackStats(true);

            /// DemoExperiment
            //DemoExperiment();
            /// DemoEmbedExperiment
            DemoEmbedExperiment();
            Console.ReadKey();
        }


        /// <summary>
        /// DemoExperiment demos how to use ExperimentRegister.Experiment. 
        /// </summary>
        public static void DemoExperiment()
        {
            for (int i = 0; i < 10; i++)
            {
                var rand = new Random().Next(0, 1000);

                Thread.Sleep(rand);
                ExperimentRegister.Experiment("Exp01", (peekResult) =>
                {
                    switch (peekResult)
                    {
                        case 'A':
                            Helloworld1();
                            return;
                        case 'B':
                            Helloworld2();
                            return;
                        case 'C':
                            Helloworld3();
                            return;
                        default:
                            return;
                    }
                });
            }
        }

        public static void DemoEmbedExperiment()
        {
            for (int i = 0; i < 20; i++)
            {
                var rand = new Random().Next(0, 1000);
                Thread.Sleep(rand);

                ExperimentRegister.Experiment("Exp01", (exp01) =>
            {
                if (exp01 == 'A')
                {
                    ExperimentRegister.Experiment("Exp02", (exp02) =>
                    {
                        Console.WriteLine("Exp01=A,Exp02=" + exp02);
                    });
                }
                else
                {
                    Console.WriteLine("Exp01=" + exp01);
                }
            });

            }
        }

        static void Helloworld1()
        {
            Console.WriteLine("Hello World 1!");

        }
        static void Helloworld2()
        {
            Console.WriteLine("Hello World 2!");

        }
        static void Helloworld3()
        {
            Console.WriteLine("Hello World 3!");
        }
    }
}
