using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleTwitter
{
	public static class Viewer
	{
		public static float maxSecondsToRunStream = 0;
		public static int maxTweetsToRunStream = 0;
		private static int tweetsReceived = 0;

		static bool running = true;

		public static event VoidEvent AppExit;

		public delegate void VoidEvent();

		public static void Start(float maxSecondsToRunStream, int maxTweetsToRunStream)
		{
			Viewer.maxSecondsToRunStream = maxSecondsToRunStream;
			Viewer.maxTweetsToRunStream = maxTweetsToRunStream;

			if (maxTweetsToRunStream > 0) {
				Stream.stream.MatchingTweetReceived += (s, a) => {
					tweetsReceived++;
					if (tweetsReceived >= maxTweetsToRunStream) {
						if (AppExit != null) {
							AppExit();
						}
					}
				};
			} else if (maxSecondsToRunStream > 0) {
				Timer t = new Timer(maxSecondsToRunStream);
				t.Start();
				t.Elapsed += (s, a) => {
					if (AppExit != null) {
						AppExit();
					}
				};
			}

			AppExit += () => {
				running = false;
			};

			// run until press escape
			while (running) {
				if (Console.KeyAvailable) {
					if (Console.ReadKey().Key == ConsoleKey.Escape) {
						break;
					}
				}
			}

			if (running) {
				if (AppExit != null) {
					AppExit();
				}
			}
			Console.ReadKey();
		}
	}
}
