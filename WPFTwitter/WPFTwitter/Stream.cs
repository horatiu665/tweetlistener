using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.Threading;

namespace WPFTwitter
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

		static Action streamThread;

		/// <summary>
		/// true when exception occurred in stream thread. used by main thread to attempt reconnect if it happens.
		/// </summary>
		static bool streamThreadException = false;

		// event triggered every time there is some error that must be logged
		public static event StreamErrorHandler Error;

		public delegate void StreamErrorHandler(string message);

		/// <summary>
		/// set twitter credentials for the app
		/// </summary>
		public static void TwitterCredentialsInit()
		{

			// "Access_Token", "Access_Token_Secret", "Consumer_Key", "Consumer_Secret"
			TwitterCredentials.ApplicationCredentials = TwitterCredentials.CreateCredentials(
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

		/// <summary>
		/// made true after running Init = only run some code once.
		/// </summary>
		private static bool _initialized = false;

		// true when stream is running.
		static bool streamRunning = false;

		/// <summary>
		/// increases every reconnect, and resets to 100 when connection is successful
		/// </summary>
		static int reconnectDelayMillis = 100;

		#region counters bullshit

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

		#endregion

		/// <summary>
		/// start stream. if not initialized, initializes = credentials, events.
		/// </summary>
		/// <param name="filter"></param>
		public static void Start(string filter)
		{
			if (streamRunning) {
				return;
			}

			if (!_initialized) {
				Stream.filter = filter;

				InitCounters();
				TwitterCredentialsInit();

				if (Error != null) {
					Error("credentials ready");
				}

				// init stream thread
				streamThread = new Action(StartStreamTask);

				// create stream
				//stream = Stream.CreateSampleStream(); // sample stream = no filter.
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
					stream.DisconnectMessageReceived += (s, a) => { CountEvent(0); };
					stream.JsonObjectReceived += (s, a) => { CountEvent(1); };
					stream.LimitReached += (s, a) => { CountEvent(2); };
					stream.StreamPaused += (s, a) => { CountEvent(3); };
					stream.StreamResumed += (s, a) => { CountEvent(4); };
					stream.StreamStarted += (s, a) => { CountEvent(5); };
					stream.StreamStopped += (s, a) => { CountEvent(6); };
					stream.TweetDeleted += (s, a) => { CountEvent(7); };
					stream.TweetLocationInfoRemoved += (s, a) => { CountEvent(8); };
					//stream.TweetReceived						+= (s, a) => { CountEvent(9); }; // for sample stream
					stream.MatchingTweetReceived += (s, a) => { CountEvent(9); }; // for filtered stream
					stream.TweetWitheld += (s, a) => { CountEvent(10); };
					stream.UnmanagedEventReceived += (s, a) => { CountEvent(11); };
					stream.UserWitheld += (s, a) => { CountEvent(12); };
					stream.WarningFallingBehindDetected += (s, a) => { CountEvent(13); };

				}
				#endregion

				if (Error != null) {
					Error("setup stream. preparing to start.");
				}

				StreamStartInsistBetter(stream);

				_initialized = true;

			} else {

				stream.ClearTracks();
				stream.AddTrack(filter);
				StreamStartInsistBetter(stream);
			}
		}

		/// <summary>
		/// Starts stream and makes sure it works.
		/// </summary>
		/// <param name="stream"></param>
		private static void StreamStartInsistBetter(Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream)
		{
			Task.Factory.StartNew(streamThread);
			//Console.WriteLine("Finished starting new Task at " + DateTime.Now.ToString());

		}

		static bool intentionalStop = false;

		/// <summary>
		/// only called from StreamStartInsist().
		/// This function should run in its own thread
		/// </summary>
		private static async void StartStreamTask()
		{
			// restart while stream throws exceptions. (not when it is closed nicely)
			while (true) {

				// log the start of the stream.
				if (Error != null) {
					Error("Stream attempting to start at time " + DateTime.Now.ToString());
				}

				// stream thread exception is only true after there is an exception in this thread
				streamThreadException = false;

				if (intentionalStop) {
					intentionalStop = false;
					break;
					
				}

				// first wait to not get banned
				await Task.Delay(reconnectDelayMillis);

				// catch exceptions within this thread. if they happen, we must retry connecting.
				try {
					await stream.StartStreamMatchingAnyConditionAsync(); // for filtered stream

				}
				catch (Exception e) {
					if (Error != null) {
						Error("Exception at StartStreamTask() thread: " + e.ToString());
					}
					// notify main thread that there was an exception here.
					streamThreadException = true;
					// wait more next time, to not get banned.
					reconnectDelayMillis *= 2;

				}


			}
		}

		public static void Stop()
		{
			if (streamRunning) {
				stream.StopStream();
				intentionalStop = true;
			}
		}

		#region events

		/// <summary>
		/// happens when stream starts. not sure if successfully or also unsuccessfully
		/// </summary>
		private static void onStreamStarted(object sender, EventArgs e)
		{
			// log start and print event args
			if (Error != null) {
				Error("OnStreamStarted event is called");
				if (e != null) {
					Error(e.ToString());
				}
			}

			streamRunning = true;

		}

		/// <summary>
		/// called when StreamStopped Tweetinvi event triggers.
		/// </summary>
		private static void onStreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
		{
			streamRunning = false;

		}

		private static void onMatchingTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
		{
			reconnectDelayMillis = 100;
		}

		private static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{

		}

		#endregion
	}
}
