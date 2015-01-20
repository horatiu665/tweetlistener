using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTwitter
{
	public static class Viewer
	{
		public static float maxSecondsToRunStream = 0;

		public static event VoidEvent AppExit;

		public delegate void VoidEvent();

		public static void Start(float maxSecondsToRunStream)
		{
			Viewer.maxSecondsToRunStream = maxSecondsToRunStream;

			// bullshit interface 2: esc to quit
			long ticksToEnd=long.MaxValue;
			if (maxSecondsToRunStream > 0) {
				ticksToEnd = DateTime.Now.AddSeconds(maxSecondsToRunStream).Ticks;
			}

			// run until press escape or ticksToEnd arrives
			while (ticksToEnd > DateTime.Now.Ticks) {
				if (Console.KeyAvailable) {
					if (Console.ReadKey().Key == ConsoleKey.Escape) {
						break;
					}
				}
			}

			if (AppExit != null) {
				AppExit();
			}
			Console.ReadKey();
		}
	}
}
