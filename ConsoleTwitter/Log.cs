using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;

namespace ConsoleTwitter
{
	/// <summary>
	/// handles listening to events and printing out logs to the console and to log files.
	/// </summary>
	public static class Log
	{
		static StreamWriter logWriter;
		public static string logPath = "log.txt";

		public static double restartLogIntervalMillis = 30000;
		static System.Timers.Timer logRestartTimer;

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
				Console.WriteLine("Path invalid. Using \"log.txt\" \nTry not to use fancy characters. Just letters and numbers");
				return "log.txt";
			}
			
		}

		/// <summary>
		/// Log module starts here
		/// </summary>
		public static void Start(string path)
		{
			logPath = ValidatePath(path);

			// open log writer
			logWriter = new StreamWriter(logPath, true);
			Output("New log started at " + DateTime.Now);
			Output("Stream running filter:");
			Output(Stream.filter);

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

			// bind to events from Stream
			#region counters
			Stream.stream.DisconnectMessageReceived		+= (s, a) => { Output(Stream.CountersString() + " - DisconnectMessageReceived		"); };
			Stream.stream.JsonObjectReceived			+= (s, a) => { Output(Stream.CountersString() + " - JsonObjectReceived				"); };
			Stream.stream.LimitReached					+= (s, a) => { Output(Stream.CountersString() + " - LimitReached					"); };
			Stream.stream.StreamPaused					+= (s, a) => { Output(Stream.CountersString() + " - StreamPaused					"); };
			Stream.stream.StreamResumed					+= (s, a) => { Output(Stream.CountersString() + " - StreamResumed					"); };
			Stream.stream.StreamStarted					+= (s, a) => { Output(Stream.CountersString() + " - StreamStarted					"); };
			Stream.stream.StreamStopped					+= (s, a) => { Output(Stream.CountersString() + " - StreamStopped					"); };
			Stream.stream.TweetDeleted					+= (s, a) => { Output(Stream.CountersString() + " - TweetDeleted					"); };
			Stream.stream.TweetLocationInfoRemoved 		+= (s, a) => { Output(Stream.CountersString() + " - TweetLocationInfoRemoved 		"); };
//			Stream.stream.TweetReceived					+= (s, a) => { Output(Stream.CountersString() + " - TweetReceived					"); }; // for sample stream
			Stream.stream.MatchingTweetReceived			+= (s, a) => { Output(Stream.CountersString() + " - MatchingTweetReceived			"); }; // for filtered stream
			Stream.stream.TweetWitheld					+= (s, a) => { Output(Stream.CountersString() + " - TweetWitheld					"); };
			Stream.stream.UnmanagedEventReceived		+= (s, a) => { Output(Stream.CountersString() + " - UnmanagedEventReceived			"); };
			Stream.stream.UserWitheld					+= (s, a) => { Output(Stream.CountersString() + " - UserWitheld						"); };
			Stream.stream.WarningFallingBehindDetected	+= (s, a) => { Output(Stream.CountersString() + " - WarningFallingBehindDetected	"); };

			#endregion

			// bind to stream errors and specific events
			Stream.Error += (s) => { Output(s); };
			Stream.stream.JsonObjectReceived += onJsonObjectReceived;
			Stream.stream.StreamStarted += onStreamStarted;
			Stream.stream.StreamStopped += onStreamStopped;

			// bind to appexit
			Viewer.AppExit += () => { 
				Output("Log stopping..."); 
				Stop();
			};

			// bind to any exception
			ExceptionHandler.WebExceptionReceived += (s, a) => {
				var exception = (Tweetinvi.Core.Exceptions.ITwitterException)a.Value;
				var statusCode = exception.StatusCode;
				Output("Web exception received. Status code " + statusCode + "\n" + exception.ToString());
			};
			
		}

		public static void Stop()
		{
			logRestartTimer.Stop();
			logWriter.Close();
			logWriter = null;

			// unbind events? test if problem maybe....
		}


		static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{
			// if e.Json is one of known types of messages, it will contain some specific keywords

			// limit = tweets were missed. the number in the Json object 
			// will be the number of tweets missed in total since the beginning of the connection.
			if (e.Json.Contains("\"limit\":")) {
				Output(
					e.Json.Substring(
						e.Json.IndexOf("track") + 7,
						e.Json.IndexOf("}") - e.Json.IndexOf("track") + 7)
					+ " tweets undelivered since connection started.");

			} else if (e.Json.Contains("\"delete\":")) {
				// tweet deleted
				Output("Tweet was deleted");

			} else if (e.Json.Contains("\"disconnect\":")) {
				Output("Stream disconnected!");
				Output(e.Json);

			} else if (e.Json.Contains("\"warning\":")) {
				Output("Twitter says warning!~!!");
				Output(e.Json);

			} else {
				// if unknown type of message, it must either be a tweet or a new type of message
			}

		}

		/// <summary>
		/// happens when stream starts
		/// </summary>
		static void onStreamStarted(object sender, EventArgs e)
		{
			Output("Stream successfully started");
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
			
		}


		
		// output funcs
		
		public static void Output()
		{
			Output("");
		}

		public static void Output(string message)
		{
			message = DateTime.Now + ": " + message;

			// showOutput message should be written even if showOutput is false.
			if (message.Contains("showOutput")) {
				Console.WriteLine(message);
				if (logWriter != null) {
					logWriter.WriteLine(message);
				}
			}

			// spam from server. heh
			if (message.Contains("<!-- Hosting24")) {
				message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
			}

			// always console and log. if you don't want this, don't run the Log module
			Console.WriteLine(message);
			
			if (logWriter != null) {
				logWriter.WriteLine(message);
			}
		}


	}
}
