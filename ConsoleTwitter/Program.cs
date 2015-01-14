using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

// for sending POST data through WebRequest
using System.IO;
using System.Net;

// for connecting directly to MySQL
using MySql.Data.MySqlClient;

namespace ConsoleTwitter
{
	class Program
	{
		/// <summary>
		/// saves latest tweet when it updates. used to print to console only at readline (to prevent insane matrix style scrolling)
		/// </summary>
		static string curTweet = "";

		/// <summary>
		/// counts events
		/// </summary>
		static List<int> counters;

		/// <summary>
		/// when true, counts events and displays them. when false, does not do those things.
		/// </summary>
		static bool countersOn = true;

		// max time to run program before it closes.
		static float maxSecondsToRunStream = 0;

		// true when stream started.
		static bool streamStarted = false;

		// connection data saved as strings
		static string localConnectionString = @"server=localhost;userid=root;password=1234;database=hivemindcloud";
		static string onlineConnectionString = @"server=mysql10.000webhost.com;userid=a3879893_admin;password=dumnezeu55;database=a3879893_tweet";
		static string localPhpTweetsLink = @"http://localhost/hhh/testing/saveTweet.php";
		static string onlinePhpTweetsLink = @"http://hivemindcloud.hostoi.com/saveTweet.php";

		static bool connectOnline = true;
		static bool saveToDatabaseOrPHP = false;

		// show output in console.
		static bool showOutput = true;

		// print output to log file.
		static bool printOutput = true;
		static StreamWriter logWriter;
		static string logPath = "log.txt";

		/// <summary>
		/// the stream used throughout the program
		/// </summary>
		static Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream;

		/// <summary>
		/// max iterations when sending to database fails. perhaps ideally we should wait a few seconds between retries. But brute force is also good sometimes.
		/// </summary>
		static int maxTweetDatabaseSendRetries = 100;
		static float secondsBetweenSendRetries = 1;

		/// <summary>
		/// init vars in the program
		/// </summary>
		static void Init()
		{
			// init stuff
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

		static void Output()
		{
			Output("");
		}

		static void Output(string message)
		{
			// showOutput message should be written even if showOutput is false.
			if (message.Contains("showOutput")) {
				Console.WriteLine(message);
				if (printOutput) {
					if (logWriter != null) {
						logWriter.WriteLine(message);
					}
				}
			}

			if (message.Contains("<!-- Hosting24")) {
				message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
			}

			if (showOutput) {
				Console.WriteLine(message);
			}
			if (printOutput) {
				if (logWriter != null) {
					logWriter.WriteLine(message);
				}
			}
		}

		/// <summary>
		/// seems that main cannot be marked async.
		/// </summary>
		static void Main(string[] args)
		{
			Init();

			TwitterCredentialsInit();

			string filter = "";
			while (filter == "") {
				Output();
				Output("-c: Connection will be made to " + (connectOnline ? "online. BEWARE: cannot send data directly with free plan!!!" : "localhost") + " . Toggle using -c command.");
				Output("-s: Data will be saved " + (saveToDatabaseOrPHP ? "directly to database" : "through PHP post request") + ". Toggle using -s command.");
				Output("-o: Console output is " + showOutput + ". Toggle using -o command.");
				Output("-l: Logging to log file is " + (printOutput ? "on" : "off") + ". Toggle using -l command");
				Output("-t " + maxSecondsToRunStream + ": Stream will run " + (maxSecondsToRunStream == 0 ? "indefinitely" : "for a maximum of " + maxSecondsToRunStream + " seconds")
					+ ". Edit by using -t <seconds> command, where <seconds> is a float. Use -t 0 for running indefinitely.");
				Output("Any other command will be interpreted as filter keywords.");
				filter = Console.ReadLine();

				if (filter == "-c") {
					connectOnline = !connectOnline;
					filter = "";
				} else if (filter == "-s") {
					saveToDatabaseOrPHP = !saveToDatabaseOrPHP;
					filter = "";
				} else if (filter == "-o") {
					showOutput = !showOutput;
					filter = "";
				} else if (filter == "-l") {
					printOutput = !printOutput;
					filter = "";
				} else if (filter.Substring(0, 2) == "-t") {
					float.TryParse(filter.Substring(3), out maxSecondsToRunStream);
					if (maxSecondsToRunStream < 0) maxSecondsToRunStream = 0;
					filter = "";
				}
			}

			if (printOutput) {
				// open log writer
				logWriter = new StreamWriter(logPath, true);
				Output("New log started at " + DateTime.Now);
				Output("Stream running filter:");
				Output(filter);

				// once in a while stop and start log, so we do not lose all the data in the log.
				System.Timers.Timer logRestartTimer = new System.Timers.Timer(30000);
				logRestartTimer.Start();
				logRestartTimer.Elapsed += (s, a) => {
					if (logWriter != null) {
						logWriter.Close();
						logWriter = new StreamWriter(logPath, true);
					} else {
						logRestartTimer.Stop();
					}
				};
				
			}

			// create stream and setup events

			//var stream = Stream.CreateSampleStream();
			//stream.TweetReceived += onTweetReceived;
			//stream.StreamStarted += onStreamStarted;

			stream = Tweetinvi.Stream.CreateFilteredStream();
			stream.StreamStarted += onStreamStarted;
			stream.StreamStopped += onStreamStopped;
			stream.MatchingTweetReceived += onMatchedTweetReceived;
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

			Output();

			// HERE: stream has started and can begin accepting tweets

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
					} else if (Console.ReadKey().Key == ConsoleKey.O) {
						showOutput = !showOutput;
						Output();
						Output("showOutput is now " + showOutput);
					}
				}
			}

			// good idea to stop stream so we don't get BANNED
			streamStarted = false;
			stream.StopStream();

			Output(" exiting.");
			Output("Press any key...");

			if (printOutput) {
				Output("Log finished successfully at " + DateTime.Now);
				logWriter.Close();
				logWriter = null;
			}
			Console.ReadKey();
		}

		static void CountEvent(int i)
		{
			counters[i]++;
			Output(CountersString());
			
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
		/// text UI in the console (oh yeah)
		/// </summary>
		static string CountersString()
		{
			string o = "Counters: ";
			for (int i = 0; i < counters.Count; i++) {
				o += (counters[i] + " ");
			}

			o += (" at time " + DateTime.Now);
			return o;
		}

		/// <summary>
		/// happens when stream starts
		/// </summary>
		static void onStreamStarted(object sender, EventArgs e)
		{
			Output("Stream successfully started");
			streamStarted = true;
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
			// reconnect here, if stream is currently started. when program stops intentionally, streamStarted is set to false.
			if (streamStarted) {
				streamStarted = false;
				StreamStartInsist(stream);

			}
		}

		/// <summary>
		/// loops until it starts the stream.
		/// </summary>
		/// <param name="stream">filtered stream to start</param>
		static void StreamStartInsist(Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream)
		{
			// used for not starting 1000 threads while waiting for stream to start async
			bool awaitingThread = false;
			while (!streamStarted) {
				try {
					// async way of doing it:
					// start stream = async operation. main method cannot be marked async
					// main method cannot be running while stream is running. that's why we make a thread. only when stream is closed this await will continue.
					if (!awaitingThread) {
						awaitingThread = true;
						Task.Factory.StartNew(async () => {
							//await stream.StartStreamAsync(); // for sample stream
							await stream.StartStreamMatchingAnyConditionAsync(); // for filtered stream
						});
					}

					// regular way, not async
					//stream.StartStreamMatchingAnyCondition();
				}
				catch (Exception e) {
					Output("Exception, could not start stream (retrying): " + e.ToString());
					awaitingThread = false;

				}
			}
		}

		/// <summary>
		/// happens when stream updates with a new tweet. use for filtered stream
		/// </summary>
		static void onMatchedTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
		{
			SendTweetToDatabase(e.Tweet);

			curTweet = e.Tweet.Text + (e.Tweet.Coordinates != null ? (" at " + e.Tweet.Coordinates.Latitude + " " + e.Tweet.Coordinates.Longitude) : "");

		}

		/// <summary>
		/// happens when stream updates with a new tweet. use for sample stream
		/// </summary>
		static void onTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.TweetReceivedEventArgs e)
		{
			curTweet = e.Tweet.Text;
		}

		/// <summary>
		/// prints an event's contents to the console. good for checking wtf is happening with strange events
		/// </summary>
		static void onGenericMessage(object sender, System.EventArgs e)
		{
			Output("Event happened: " + e);
		}

		/// <summary>
		/// sends tweet to database via chosen method
		/// </summary>
		/// <param name="tweet"></param>
		static void SendTweetToDatabase(Tweetinvi.Core.Interfaces.ITweet tweet, int retries = 0)
		{
			if (saveToDatabaseOrPHP) {
				SaveToDatabase(tweet);

			} else {
				try {
					SaveToPhpFullTweet(tweet);
				}
				catch (Exception e) {
					Output(e.ToString());
					// retry maxTweetDatabaseSendRetries times to send tweet to database; if error, this might help.
					if (retries < maxTweetDatabaseSendRetries) {
						Task.Factory.StartNew(() => {
							// wait a little and then try again
							var ticksToWait = DateTime.Now.AddSeconds(secondsBetweenSendRetries).Ticks;
							while (ticksToWait > DateTime.Now.Ticks) { /* do nothing */ }
							SendTweetToDatabase(tweet, retries + 1);

						});
					} else {
						Output("Failed to send after " + retries + " tries");
					}
				}
			}
		}

		/// <summary>
		/// saves tweet data to another table called "tweets"
		/// </summary>
		/// <param name="tweet"></param>
		static void SaveToPhpFullTweet(Tweetinvi.Core.Interfaces.ITweet tweet)
		{
			// Create a request using a URL that can receive a post (link.php)
			WebRequest request = WebRequest.Create(connectOnline ? onlinePhpTweetsLink : localPhpTweetsLink);
			// Set the Method property of the request to POST.
			request.Method = "POST";
			// Create POST data and convert it to a byte array. 
			// also encode tweet to avoid problems with "&"
			string postData = "tweet=" + WebUtility.UrlEncode(tweet.Text);
			
			byte[] byteArray = Encoding.UTF8.GetBytes(postData);
			// Set the ContentType property of the WebRequest.
			request.ContentType = "application/x-www-form-urlencoded";
			// Set the ContentLength property of the WebRequest.
			request.ContentLength = byteArray.Length;
			// Get the request stream.
			System.IO.Stream dataStream = request.GetRequestStream();
			// Write the data to the request stream.
			dataStream.Write(byteArray, 0, byteArray.Length);
			// Close the Stream object.
			dataStream.Close();

			// optional response
			// Get the response.
			WebResponse response = request.GetResponse();
			// Display the status.
			Output(((HttpWebResponse)response).StatusDescription);
			// Get the stream containing content returned by the server.
			dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();
			// Display the content.
			Output(responseFromServer);
			// Clean up the streams.
			reader.Close();
			dataStream.Close();
			response.Close();
			
		}

		/// <summary>
		/// saves directly to database.
		/// </summary>
		/// <param name="message"></param>
		static void SaveToDatabase(Tweetinvi.Core.Interfaces.ITweet tweet)
		{
			string connectionString = connectOnline ? onlineConnectionString : localConnectionString;

			try {
				MySqlConnection connection = new MySqlConnection(connectionString);
				connection.Open();
				Output("Connection to " + connectionString + " opened successfully");
				

				string query = "";
				#region setup  query

				// raw tweet
				string rawTweet = tweet.Text;
				
				// hashtags as given by raw tweet data from Twitter API
				List<string> hashtags = tweet.Hashtags.Select<Tweetinvi.Core.Interfaces.Models.Entities.IHashtagEntity, string>(h => h.Text).ToList();
				
				// tweet split into words
				List<string> wordList = tweet.Text.Split(' ').ToList();
				
				// the actual query
				query = " INSERT INTO ";
				query += " `wordscount` ";
				query += " (`id`, `word`, `count`) ";
				query += " VALUES ";
				query += " (NULL, '" + wordList[0] + "', '1', " + rawTweet + ", " + DateTime.Now + ") ";
				for (int i = 1; i < wordList.Count; i++) {
					query += " ,(NULL, '" + wordList[i] + "', '1', " + rawTweet + ", " + DateTime.Now + ") ";
				}
				// if duplicate key (the word),increase count instead of new row
				query += " ON DUPLICATE KEY UPDATE ";
				query += " count = count + 1 ";

				#endregion

				MySqlCommand command = new MySqlCommand(query, connection);
				command.CommandType = System.Data.CommandType.Text;
				// timeout in seconds
				command.CommandTimeout = 60;

				var reader = command.ExecuteReader();

				while (reader.Read()) {
					foreach (var r in reader) {
						Output(r.ToString());
					}
				}

				connection.Close();

			}
			catch (MySqlException e) {

				if (e.Number == 1045) {
					Output("Invalid username or password");
				} else {
					Output(e.ToString());
				}

			}
		}

	}
}
