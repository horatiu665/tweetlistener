using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;
using Newtonsoft.Json.Linq;

namespace WPFTwitter
{
	/// <summary>
	/// handles listening to events and printing out logs to the console and to log files.
	/// </summary>
	public static class Log
	{
		private static bool _started = false;

		public static bool Started
		{
			get { return Log._started; }
		}

		static StreamWriter logWriter;
		public static string logPath = "log.txt";
		public static double restartLogIntervalMillis = 30000;
		static System.Timers.Timer logRestartTimer;

		static StreamWriter smallLogWriter;
		public static string smallLogPath = "smalllog.txt";
		public static double restartSmallLogIntervalMillis = 30037;
		static System.Timers.Timer smallLogRestartTimer;

		public delegate void LogOutputEventHandler(string message);
		public static event LogOutputEventHandler LogOutput, SmallLogOutput;


		/// <summary>
		/// returns a valid version of the path given
		/// </summary>
		/// <param name="path">path to validate</param>
		/// <returns>valid path that can be used</returns>
		public static string ValidatePath(string path)
		{
			try {
				string fullPath = Path.GetFullPath(path);
				// if no error, path is valid
				if (path.EndsWith(".txt")) {
					return path;
				} else {
					return path + ".txt";
				}
			}
			catch {
				Output("Path invalid. Using \"log.txt\". Try not to use fancy characters. Just letters and numbers");
				return "log.txt";
			}

		}

		/// <summary>
		/// Log module starts here
		/// </summary>
		public static void Start(string path, bool outputCounters = true, bool databaseMessages = true)
		{
			if (!_started) {

				logPath = ValidatePath(path);

				// open log writer
				logWriter = new StreamWriter(logPath, true);
				Output("New log started");
				Output("Stream running filter:");
				Output(Stream.filter);

				// small log init
				smallLogWriter = new StreamWriter(smallLogPath, true);
				SmallOutput("New log started");
				SmallOutput("Stream running filter:");
				SmallOutput(Stream.filter);

				// once in a while stop and start log, so we do not lose all the data in the log.
				logRestartTimer = new System.Timers.Timer(restartLogIntervalMillis);
				logRestartTimer.Start();
				logRestartTimer.Elapsed += (s, a) => {
					if (logWriter != null) {
						logWriter.Close();
						logWriter = new StreamWriter(logPath, true);
					} else {
						logRestartTimer.Stop();
					}
				};

				// same thing for small log
				smallLogRestartTimer = new System.Timers.Timer(restartSmallLogIntervalMillis);
				smallLogRestartTimer.Start();
				smallLogRestartTimer.Elapsed += (s, a) => {
					if (smallLogWriter != null) {
						smallLogWriter.Close();
						smallLogWriter = new StreamWriter(smallLogPath, true);
					} else {
						smallLogRestartTimer.Stop();
					}
				};

				if (outputCounters) {
					// bind to events from Stream
					#region counters
					Stream.stream.DisconnectMessageReceived
						+= (s, a) => { Output(Stream.CountersString() + " - DisconnectMessageReceived		"); };
					Stream.stream.JsonObjectReceived
						+= (s, a) => { Output(Stream.CountersString() + " - JsonObjectReceived				"); };
					Stream.stream.LimitReached
						+= (s, a) => { Output(Stream.CountersString() + " - LimitReached					"); };
					Stream.stream.StreamPaused
						+= (s, a) => { Output(Stream.CountersString() + " - StreamPaused					"); };
					Stream.stream.StreamResumed
						+= (s, a) => { Output(Stream.CountersString() + " - StreamResumed					"); };
					Stream.stream.StreamStarted
						+= (s, a) => { Output(Stream.CountersString() + " - StreamStarted					"); };
					Stream.stream.StreamStopped
						+= (s, a) => { Output(Stream.CountersString() + " - StreamStopped					"); };
					Stream.stream.TweetDeleted
						+= (s, a) => { Output(Stream.CountersString() + " - TweetDeleted					"); };
					Stream.stream.TweetLocationInfoRemoved
						+= (s, a) => { Output(Stream.CountersString() + " - TweetLocationInfoRemoved 		"); };
					// Stream.stream.TweetReceived	
					//	+= (s, a) => { Output(Stream.CountersString() + " - TweetReceived					"); }; // for sample stream
					Stream.stream.MatchingTweetReceived
						+= (s, a) => { Output(Stream.CountersString() + " - MatchingTweetReceived			"); }; // for filtered stream
					Stream.stream.TweetWitheld
						+= (s, a) => { Output(Stream.CountersString() + " - TweetWitheld					"); };
					Stream.stream.UnmanagedEventReceived
						+= (s, a) => { Output(Stream.CountersString() + " - UnmanagedEventReceived			"); };
					Stream.stream.UserWitheld
						+= (s, a) => { Output(Stream.CountersString() + " - UserWitheld						"); };
					Stream.stream.WarningFallingBehindDetected
						+= (s, a) => { Output(Stream.CountersString() + " - WarningFallingBehindDetected	"); };

					#endregion
				}

				// bind to stream errors and specific events
				Stream.Error += (s) => { Output(s); SmallOutput(s); };
				Stream.stream.JsonObjectReceived += onJsonObjectReceived;
				Stream.stream.StreamStarted += onStreamStarted;
				Stream.stream.StreamStopped += onStreamStopped;
				Stream.stream.LimitReached += onLimitReached;

				// bind to any exception
				//ExceptionHandler.WebExceptionReceived += (s, a) => {
				//	var exception = (Tweetinvi.Core.Exceptions.ITwitterException)a.Value;
				//	var statusCode = exception.StatusCode;
				//	if (exception != null && statusCode != null) {
				//		Output("Web exception received. Status code " + statusCode + "\n" + exception.ToString());
				//	}
				//};

				// database messages only for big log
				if (databaseMessages) {
					DatabaseSaver.Message += (s) => {
						Output(s);
					};
				}

				_started = true;
			}

		}

		private static void onLimitReached(object sender, Tweetinvi.Core.Events.EventArguments.LimitReachedEventArgs e)
		{
			SmallOutput("Tweets missed: " + e.NumberOfTweetsNotReceived.ToString());
		}

		public static void Stop()
		{
			// too dangerous to stop log, because events are not cleaned up, and restarting it would mean double the prints.

			//logRestartTimer.Stop();
			//logWriter.Close();
			//logWriter = null;

			//smallLogRestartTimer.Stop();
			//smallLogWriter.Close();
			//smallLogWriter = null;

			//// unbind events? test if problem maybe....

			//_started = false;
		}

		/// <summary>
		/// use in EvaluateJsonChildren() to print out the fields which might give useful information in the log
		/// </summary>
		static string[] interestingFields = {
			"limit", "disconnect", "warning", "delete",

			"text"
		};

		static void EvaluateJsonChildren(JToken j)
		{
			string s = "Fields in json object: \n";
			foreach (var j2 in j) {
				var p = j2 as JProperty;
				if (p != null) {
					s += p.Name;
					if (interestingFields.Contains(p.Name)) {
						s += ":" + p.Value;
					}
					s += ", ";
				}
			}
			Output(s);

			/*
			 * jsonRoot: {
			 *		"key1": {
			 *			"key11" : "value11",
			 *			"key12" : {
			 *				"key121" : "value121";
			 *			},
			 *			"key13" : "value13"
			 *		},
			 *		"key2" : "value2"
			 * }
			*/
		}

		static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{
			JObject json = JObject.Parse(e.Json);

			EvaluateJsonChildren(json);
		}

		/// <summary>
		/// happens when stream starts
		/// </summary>
		static void onStreamStarted(object sender, EventArgs e)
		{
			Output("Stream successfully started");
			SmallOutput("Stream successfully started");
		}

		/// <summary>
		/// called when StreamStopped Tweetinvi event triggers.
		/// </summary>
		private static void onStreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
		{
			Output("Stream disconnected.");
			if (e.DisconnectMessage != null) {
				Output("Message code: " + e.DisconnectMessage.Code);
				if (e.DisconnectMessage.Reason != null) {
					Output("Reason: " + e.DisconnectMessage.Reason);
				}
			}
			if (e.Exception != null) {
				Output("Exception: " + e.Exception.ToString());
			}

			// clone for small output
			SmallOutput("Stream disconnected.");
			if (e.DisconnectMessage != null) {
				SmallOutput("Message code: " + e.DisconnectMessage.Code);
				if (e.DisconnectMessage.Reason != null) {
					SmallOutput("Reason: " + e.DisconnectMessage.Reason);
				}
			}
			if (e.Exception != null) {
				SmallOutput("Exception: " + e.Exception.ToString());
			}

		}



		// output funcs

		public static void Output()
		{
			Output("");
		}

		public static void Output(string message)
		{
			try {
				if (LogOutput != null) {
					LogOutput(message);
				}

				message = DateTime.Now + ": " + message;

				// spam from server. heh
				if (message.Contains("<!-- Hosting24")) {
					message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
				}

				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(message);
				}

				if (logWriter != null) {
					logWriter.WriteLine(message);
				}
			}
			catch (Exception e) {
				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(e.ToString());
				}
			}
		}

		public static void SmallOutput(string message)
		{
			try {

				if (SmallLogOutput != null) {
					SmallLogOutput(message);
				}

				message = DateTime.Now + ": " + message;

				// spam from server. heh
				if (message.Contains("<!-- Hosting24")) {
					message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
				}

				if (smallLogWriter != null) {
					smallLogWriter.WriteLine(message);
				}
			}
			catch (Exception e) {
				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
