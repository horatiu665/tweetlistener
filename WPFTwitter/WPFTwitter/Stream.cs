using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.Threading;
using System.IO;
using System.Collections.ObjectModel;

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

		private static List<string> credentials = new List<string>() { "", "", "", "" };

		public static event Action<List<string>> CredentialsChange;

		/// <summary>
		/// set twitter credentials for the app
		/// </summary>
		public static void TwitterCredentialsInit(List<string> creds = null)
		{
			// if creds == null, init was called for reset (from file or hardcoded).
			if (creds == null) {

				// if .ini file found, take creds from there. else hardcode them.
				bool credsFoundInFile = false;
				if (File.Exists("config.ini")) {
					var jsonRead = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.ini"));
					credentials = new List<string>() {
						jsonRead["Access_Token"].ToString(),
						jsonRead["Access_Token_Secret"].ToString(),
						jsonRead["Consumer_Key"].ToString(),
						jsonRead["Consumer_Secret"].ToString()
					};
					if (credentials[0] != null && credentials[1] != null && credentials[2] != null && credentials[3] != null) {
						credsFoundInFile = true;
					}
				}

				if (!credsFoundInFile) {
					// HARD CODE the creds
					credentials = new List<string>() {
						"2504893657-b30BlFnSdCKo42LFIyWKbywseTq2PyG0StdpKp6",
						"JDCAI46G4qDWHPYLjfhuM9iDll53wgRwXZKhcMkw84dwi",
						"s3QKQous2rgkpglkSTRHQz9dw",
						"t9cZGT3Rcheh8742LVZHaIc5uvLsSXSGvqUb3NIGr9WMt097IH"
					};

					var jsonWrite = new Newtonsoft.Json.Linq.JObject();
					jsonWrite.Add("Access_Token",
						new Newtonsoft.Json.Linq.JValue(credentials[0]));
					jsonWrite.Add("Access_Token_Secret",
						new Newtonsoft.Json.Linq.JValue(credentials[1]));
					jsonWrite.Add("Consumer_Key",
						new Newtonsoft.Json.Linq.JValue(credentials[2]));
					jsonWrite.Add("Consumer_Secret",
						new Newtonsoft.Json.Linq.JValue(credentials[3]));
					StreamWriter sw = new StreamWriter("config.ini");
					sw.Write(jsonWrite.ToString());
					sw.Close();
				}

				if (CredentialsChange != null) {
					CredentialsChange(credentials);
				}
			} else {
				// set credentials to creds
				if (creds[0] != null && creds[1] != null && creds[2] != null && creds[3] != null) {
					credentials = new List<string>(creds);
				}
			}

			TwitterCredentials.ApplicationCredentials = TwitterCredentials.CreateCredentials(
				// "Access_Token"
				credentials[0],
				// "Access_Token_Secret"
				credentials[1],
				// "Consumer_Key"
				credentials[2],
				// "Consumer_Secret"
				credentials[3]
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

				// stream thread exception is only true after there is an exception in this thread
				streamThreadException = false;

				if (intentionalStop) {
					intentionalStop = false;
					break;

				}

				// log the start of the stream.
				if (Error != null) {
					Error("Stream attempting to start at time " + DateTime.Now.ToString());
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
			
			streamRunning = false;

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
