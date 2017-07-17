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

			Console.ReadKey();
		}

		public static void Helloworld1()
		{
			Console.WriteLine("Hello World 1!");

		}
		public static void Helloworld2()
		{
			Console.WriteLine("Hello World 2!");

		}
		public static void Helloworld3()
		{
			Console.WriteLine("Hello World 3!");

		}
	}
}
