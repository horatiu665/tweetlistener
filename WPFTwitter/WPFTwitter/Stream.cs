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

		// true when stream started.
		static bool streamStarted = false;

		/// <summary>
		/// increases every reconnect, and resets to 100 when connection is successful
		/// </summary>
		static int reconnectDelayMillis = 100;

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

		private static bool _initialized = false;

		public static void Init(string filter)
		{
			if (!_initialized) {
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

				//StreamStartInsist(stream);
				StreamStartInsistBetter(stream);

				_initialized = true;
			} else {
				Start(filter);
			}
		}

		/// <summary>
		/// restarts stream after it being initialized.
		/// </summary>
		/// <param name="filter"></param>
		public static void Start(string filter)
		{
			if (!_initialized) {
				Init(filter);
			} else {

				stream.ClearTracks();
				stream.AddTrack(filter);
				//StreamStartInsist(stream);
				StreamStartInsistBetter(stream);
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
						// main thread cannot be running while stream is running. that's why we make a new thread. this await will continue only when stream is closed.
						if (!awaitingThread || streamThreadException) {
							awaitingThread = true;
							streamThreadException = false;

							// interesting tip: exceptions thrown in the new thread cause the thread to end but the main thread still runs.
							// Therefore, previously, when the stream would disconnect and perhaps there was an error in Tweetinvi, the thread would close
							// but the program would never know the thread closed and it would continue to run indefinitely,
							// waiting for "awaitingThread" to become false, or for "streamStarted" to become true
							// solution: check for exceptions within new thread, and solve them inside,
							// then communicate somehow with the main thread, and update status when the inner thread throws an exception.
							Task.Factory.StartNew(() => { StartStreamTask(); });

						}
					} else {
						#region not async
						// regular way, not async, but still threaded
						if (!awaitingThread) {
							awaitingThread = true;
							Task.Factory.StartNew(() => {
								Task.Delay(reconnectDelayMillis);
								//await stream.StartStreamAsync(); // for sample stream
								stream.StartStreamMatchingAnyCondition(); // for filtered stream

							});
						}
						#endregion
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

		/// <summary>
		/// true when exception occurred in stream thread. used by main thread to attempt reconnect if it happens.
		/// </summary>
		static bool streamThreadException = false;

		/// <summary>
		/// only called from StreamStartInit().
		/// This function should run in its own thread
		/// </summary>
		private static async void StartStreamTask()
		{
			// stream thread exception is only true after there is an exception in this thread
			streamThreadException = false;

			// first wait to not get banned
			await Task.Delay(reconnectDelayMillis);
			
			// catch exceptions within this thread, and communicate them to the main thread.
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

		static string streamStatus = "none";

		private static void StreamStartInsistBetter(Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream)
		{
			// initial state: not waiting for thread.
			bool awaitingThread = false;
			// while stream not started, try to start it.
			while (!streamStarted) {
				// if not waiting for thread, or if there was an exception
				if (!awaitingThread || streamThreadException) {
					// we are waiting for stream to start.
					awaitingThread = true;
					// try to start thread (try catch inside new thread; communication with this thread through variable streamThreadException).
					Task.Factory.StartNew(() => { StartStreamTask(); });

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
				//StreamStartInsist(stream);
				StreamStartInsistBetter(stream);
			}

		}

		private static void onMatchingTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
		{
			reconnectDelayMillis = 100;
		}

		private static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{

		}

	}
}
