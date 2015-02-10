using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using MySql.Data.MySqlClient;

namespace ConsoleTwitter
{
	/// <summary>
	/// connects to the twitter events in the Stream and saves tweets to database in various methods.
	/// </summary>
	public static class DatabaseSaver
	{

		public static bool connectOnline;
		public static bool saveToDatabaseOrPhp;

		// connection data saved as strings
		public static string localConnectionString = @"server=localhost;userid=root;password=1234;database=hivemindcloud";
		public static string onlineConnectionString = @"server=mysql10.000webhost.com;userid=a3879893_admin;password=dumnezeu55;database=a3879893_tweet";
		public static string localPhpTweetsLink = @"http://localhost/hhh/testing/saveTweet.php";
		public static string onlinePhpTweetsLink = @"http://hivemindcloud.hostoi.com/saveTweet.php";
		public static string localPhpJsonLink = @"http://localhost/hhh/testing/php/saveJson.php";
		public static string onlinePhpJsonLink = @"http://hivemindcloud.hostoi.com/saveJson.php";

		/// <summary>
		/// max iterations when sending to database fails. perhaps ideally we should wait a few seconds between retries. But brute force is also good sometimes.
		/// </summary>
		static int maxTweetDatabaseSendRetries = 100;
		static float secondsBetweenSendRetries = 1;

		// event triggered every time there is some error that must be logged
		public static event DatabaseSaverMessage Message;

		public delegate void DatabaseSaverMessage(string message);

		/// <summary>
		/// starts database module which binds to receive tweet event, and saves data to database.
		/// </summary>
		public static void Start(bool connectOnline, bool saveToDatabaseOrPhp)
		{
			DatabaseSaver.connectOnline = connectOnline;
			DatabaseSaver.saveToDatabaseOrPhp = saveToDatabaseOrPhp;

			Stream.stream.MatchingTweetReceived += onMatchedTweetReceived;
			Stream.stream.JsonObjectReceived += onJsonObjectReceived;
		}

		private static void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
		{
			SendJsonToDatabase(e.Json);
		}

		/// <summary>
		/// happens when stream updates with a new tweet. use for filtered stream
		/// </summary>
		static void onMatchedTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
		{
			//SendTweetToDatabase(e.Tweet);

		}

		static void SendJsonToDatabase(string json)
		{
			if (saveToDatabaseOrPhp) {
				Console.WriteLine("Send to database not supported, only PHP");
			} else {
				SaveToPhpFullJson(json);

			}
		}

		static void SaveToPhpFullJson(string json)
		{
			// Create a request using a URL that can receive a post (link.php)
			WebRequest request = WebRequest.Create(connectOnline ? onlinePhpJsonLink : localPhpJsonLink);
			// Set the Method property of the request to POST.
			request.Method = "POST";
			// Create POST data and convert it to a byte array. 
			// also encode tweet to avoid problems with "&"
			string postData = "json=" + WebUtility.UrlEncode(json);
			//Console.WriteLine(postData);

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
			if (Message != null) {
				Message(((HttpWebResponse)response).StatusDescription);
			}
			// Get the stream containing content returned by the server.
			dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();
			// Display the content.
			if (Message != null) {
				Message(responseFromServer);
				
			}
			// Clean up the streams.
			reader.Close();
			dataStream.Close();
			response.Close();

		}


		/// <summary>
		/// sends tweet to database via chosen method
		/// </summary>
		/// <param name="tweet"></param>
		static void SendTweetToDatabase(Tweetinvi.Core.Interfaces.ITweet tweet, int retries = 0)
		{
			if (saveToDatabaseOrPhp) {
				SaveToDatabase(tweet);

			} else {
				try {
					SaveToPhpFullTweet(tweet);
				}
				catch (Exception e) {
					if (Message != null) {
						Message(e.ToString());
					}
					// retry maxTweetDatabaseSendRetries times to send tweet to database; if error, this might help.
					if (retries < maxTweetDatabaseSendRetries) {
						Task.Factory.StartNew(() => {
							// wait a little and then try again
							var ticksToWait = DateTime.Now.AddSeconds(secondsBetweenSendRetries).Ticks;
							while (ticksToWait > DateTime.Now.Ticks) { /* do nothing */ }
							SendTweetToDatabase(tweet, retries + 1);

						});
					} else {
						if (Message != null) {
							Message("Failed to send after " + retries + " tries");
						}
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
			if (Message != null) {
				Message(((HttpWebResponse)response).StatusDescription);
			}
			// Get the stream containing content returned by the server.
			dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();
			// Display the content.
			if (Message != null) {
				Message(responseFromServer);
			}
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
				if (Message != null) {
					Message("Connection to " + connectionString + " opened successfully");
				}

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
						if (Message != null) {
							Message(r.ToString());
						}
					}
				}

				connection.Close();

			}
			catch (MySqlException e) {

				if (e.Number == 1045) {
					if (Message != null) {
						Message("Invalid username or password");
					}
				} else {
					if (Message != null) {
						Message(e.ToString());

					}
				}

			}
		}

	}
}
