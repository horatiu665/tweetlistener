using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.Threading;

namespace ConsoleTwitter
{
	/// <summary>
	/// handles twitter stream connection (and reconnection) and tweet receiving events.
	/// </summary>
	public static class Stream
	{
		/// <summary>
		/// the filter used for the stream
		/// </summary>
		public static string filter = "";
		
		/// <summary>
		/// the stream used throughout the program
		/// </summary>
		public static Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream;

		// true when stream started.
		static bool streamStarted = false;

		/// <summary>
		/// increases every reconnect, and resets to 100 when connection is successful
		/// </summary>
		static int reconnectDelayMillis = 1000;

		// event triggered every time there is some error that must be logged
		public static event StreamErrorHandler Error;

		public delegate void StreamErrorHandler(string message);

		/// <summary>
		/// when true, counts events and displays them. when false, does not do those things.
		/// </summary>
		static bool countersOn = true;

		/// <summary>
		/// counts events
		/// </summary>
		static List<int> counters;

		/// <summary>
		/// init counters array which counts events from Streaming
		/// </summary>
		static void InitCounters()
		{
			if (countersOn) {
				counters = new List<int>();
				for (int i = 0; i < 14; i++) {
					counters.Add(0);
				}
			}
		}

		/// <summary>
		/// set twitter credentials for the app
		/// </summary>
		static void TwitterCredentialsInit()
		{

			// "Access_Token", "Access_Token_Secret", "Consumer_Key", "Consumer_Secret"
			TwitterCredentials.SetCredentials(
				// "Access_Token"
				"2504893657-txY29l8THkV9NhiR0ErmfXVHoSdp9RuvrhAN2DN",
				// "Access_Token_Secret"
				"TZ82eeDtaW5cLJybm4nfsmRXITz8qeCU0Y9LhHN6J5bWn",
				// "Consumer_Key"
				"hpOr7rLKQU98zTzfu2G9Qavbd",
				// "Consumer_Secret"
				"uz5PC6S6M5rTpvyviZ3pBf2UCp6Ih4ALj1EN4D6T2svCD7d15y"
				);

		}

		public static void Start(string filter)
		{
			Stream.filter = filter;

			InitCounters();
			TwitterCredentialsInit();
	
			// create stream

			//stream = Stream.CreateSampleStream();
			stream = Tweetinvi.Stream.CreateFilteredStream();

			// setup events
			stream.StreamStarted += onStreamStarted;
			stream.StreamStopped += onStreamStopped;
			stream.MatchingTweetReceived += onMatchingTweetReceived;
			stream.JsonObjectReceived += onJsonObjectReceived;

			////////////// setup stream filter
			stream.AddTrack(filter);

			#region counters
			// to see wtf is going on. also this is a list of all events possible for the stream. useful at a glance.
			if (countersOn) {
				stream.DisconnectMessageReceived			+= (s, a) => { CountEvent(0); };
				stream.JsonObjectReceived					+= (s, a) => { CountEvent(1); };
				stream.LimitReached							+= (s, a) => { CountEvent(2); };
				stream.StreamPaused							+= (s, a) => { CountEvent(3); };
				stream.StreamResumed						+= (s, a) => { CountEvent(4); };
				stream.StreamStarted						+= (s, a) => { CountEvent(5); };
				stream.StreamStopped						+= (s, a) => { CountEvent(6); };
				stream.TweetDeleted							+= (s, a) => { CountEvent(7); };
				stream.TweetLocationInfoRemoved				+= (s, a) => { CountEvent(8); };
				//stream.TweetReceived						+= (s, a) => { CountEvent(9); }; // for sample stream
				stream.MatchingTweetReceived				+= (s, a) => { CountEvent(9); }; // for filtered stream
				stream.TweetWitheld							+= (s, a) => { CountEvent(10); };
				stream.UnmanagedEventReceived				+= (s, a) => { CountEvent(11); };
				stream.UserWitheld							+= (s, a) => { CountEvent(12); };
				stream.WarningFallingBehindDetected			+= (s, a) => { CountEvent(13); };

			}
			#endregion

			StreamStartInsist(stream);
			//StreamStartInsistBetter(stream);

			Viewer.AppExit += Stop;

		}

		/// <summary>
		/// counts event number i. to find which event that is, consult the list in the Start() function.
		/// </summary>
		/// <param name="i"></param>
		private static void CountEvent(int i)
		{
			counters[i]++;
			
		}

		/// <summary>
		/// returns string of event count, useful for logging and debugging
		/// </summary>
		public static string CountersString()
		{
			if (!countersOn) {
				return "Counters: off";
			}

			string o = "Counters: ";
			for (int i = 0; i < counters.Count; i++) {
				o += (counters[i] + " ");
			}

			return o;
		}


		/// <summary>
		/// loops until it starts the stream.
		/// </summary>
		/// <param name="stream">filtered stream to start</param>
		private static void StreamStartInsist(Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream)
		{
			// used for not starting 1000 threads while waiting for stream to start async
			bool awaitingThread = false;
			while (!streamStarted) {
				try {
					bool asyncMethod = true;

					// choose async or sync connection. both using threads.
					if (asyncMethod) {
						// async way
						// start stream = async operation. main method cannot be marked async
						// main thread cannot be running while stream is running. that's why we make a new thread. only when stream is closed this await will continue.
						if (!awaitingThread) {
							awaitingThread = true;
							Task.Factory.StartNew(async () => {
								await Task.Delay(reconnectDelayMillis);
								try {
									//await stream.StartStreamAsync(); // for sample stream
									await stream.StartStreamMatchingAnyConditionAsync(); // for filtered stream
								}
								catch (Exception e) {
									Console.WriteLine(e.ToString());
								}
							});
						}
					} else {
						// regular way, not async, but still threaded
						if (!awaitingThread) {
							awaitingThread = true;
							Task.Factory.StartNew(() => {
								Task.Delay(reconnectDelayMillis);
								//await stream.StartStreamAsync(); // for sample stream
								stream.StartStreamMatchingAnyCondition(); // for filtered stream

							});
						}
					}
				}
				catch (Exception e) {
					if (Error != null) {
						Error("Exception, could not start stream (retrying): " + e.ToString());
					}
					awaitingThread = false;
					// increase delay between reconnect attempts, because we could get banned
					reconnectDelayMillis *= 2;

				}
			}
		}
		
		static string streamStatus = "none";

		private static void StreamStartInsistBetter(Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream)
		{
			Console.WriteLine("///////////Starting stream better");
			// try to start stream until we succeed
			while (streamStatus != "successfully started") {
				Console.WriteLine(streamStatus);
				Thread.Sleep(1000);
				try {
					// only try again if we are not currently trying to start a stream.
					if (streamStatus != "waiting to start") {
						Console.WriteLine("//////////////waiting to start is now true");
						streamStatus = "waiting to start";
						Task.Factory.StartNew(() => {
							Console.WriteLine("////////////////Start delay");
							Task.Delay(reconnectDelayMillis);
							Console.WriteLine("//////////Finish delay. start await for async");
							//await stream.StartStreamAsync(); // for sample stream
							stream.StartStreamMatchingAnyCondition(); // for filtered stream
							streamStatus = "successfully started";
							Console.WriteLine("/////////////////Successfully started");
						});
					}
				}
				catch (Exception e) {
					if (Error != null) {
						Error("Exception, could not start stream (retrying): " + e.ToString());
					}
					streamStatus = "error";
					Console.WriteLine("/////////////////////Error happened.trying again I guess");
					reconnectDelayMillis *= 2;
				}
			}
		}

		public static void Stop()
		{
			streamStarted = false;
			stream.StopStream();
		}


		/// <summary>
		/// happens when stream starts. not sure if successfully or also unsuccessfully
		/// </summary>
		private static void onStreamStarted(object sender, EventArgs e)
		{
			streamStarted = true;
			
		}

		/// <summary>
		/// called when StreamStopped Tweetinvi event triggers.
		/// </summary>
		private static void onStreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
		{
			// reconnect here, if stream is currently started. when program stops intentionally, streamStarted is set to false.
			if (streamStarted) {
				streamStarted = false;
				StreamStartInsist(stream);
			//	StreamStartInsistBetter(stream);
			}

		}

		private static void onMatchingTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
		{
			reconnectDelayMillis = 1000;
		}

		private static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{
			
		}

	}
}
