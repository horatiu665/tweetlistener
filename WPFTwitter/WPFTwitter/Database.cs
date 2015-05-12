﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.Interfaces;

namespace WPFTwitter
{
	/// <summary>
	/// connects to the twitter events in the Stream and saves tweets to database in various methods.
	/// </summary>
	public class DatabaseSaver
	{
		private Log log;

		public DatabaseSaver(Log l)
		{
			log = l;

			// set up events
			Message += (s) => { log.Output(s); };
		}

		private bool _started = false;

		public bool connectOnline;
		public bool saveToDatabaseOrPhp;

		// connection data saved as strings
		public string localConnectionString = @"server=localhost;userid=root;password=1234;database=hivemindcloud";
		public string onlineConnectionString = @"server=mysql10.000webhost.com;userid=a3879893_admin;password=dumnezeu55;database=a3879893_tweet";
		public string localPhpJsonLink = @"http://localhost/tweetlistener/php/saveJson.php";
		public string onlinePhpJsonLink = @"http://hivemindcloud.hostoi.com/saveJson.php";

		public string textFileDatabasePath = "rawJsonBackup.txt";

		/// <summary>
		/// max iterations when sending to database fails. perhaps ideally we should wait a few seconds between retries. But brute force is also good sometimes.
		/// </summary>
		int maxTweetDatabaseSendRetries = 100;

		public int MaxTweetDatabaseSendRetries
		{
			get { return maxTweetDatabaseSendRetries; }
			set { maxTweetDatabaseSendRetries = value; }
		}
		float secondsBetweenSendRetries = 1;

		// event triggered every time there is some error that must be logged
		public event DatabaseSaverMessage Message;

		public delegate void DatabaseSaverMessage(string message);

		/// <summary>
		/// starts database module which binds to receive tweet event, and saves data to database.
		/// </summary>
		public void Start(bool connectOnline, bool saveToDatabaseOrPhp)
		{
			if (!_started) {
				this.connectOnline = connectOnline;
				this.saveToDatabaseOrPhp = saveToDatabaseOrPhp;

				// DO NOT CREATE DEPENDENCY FROM DATABASE TO STREAM. DO IT THE OTHER WAY AROUND
				//stream.stream.MatchingTweetReceived += onMatchedTweetReceived;
				//stream.stream.JsonObjectReceived += onJsonObjectReceived;



				_started = true;

			}
		}

		public void SaveJson(string json)
		{
			if (saveToDatabaseOrPhp) {
				if (Message != null) {
					Message("Send to database not supported, only PHP");
				}
			} else {
				SaveToPhpFullJson(json);
				SaveToTextFile(json);
			}
		}

		/// <summary>
		/// sends tweet to database via chosen method
		/// </summary>
		/// <param name="tweet"></param>
		public void SaveTweet(ITweet tweet, int retries = 0)
		{
			if (saveToDatabaseOrPhp) {
				SaveToDatabase(tweet);

			} else {
				try {
					// encode tweet to json and use SaveToPhpFullJson(string json).
					JObject json = EncodeTweetToJson(tweet);

					SaveToPhpFullJson(json.ToString());
					SaveToTextFile(json.ToString());

				}
				catch (Exception e) {
					if (Message != null) {
						Message(e.ToString());
					}
					// retry maxTweetDatabaseSendRetries times to send tweet to database; if error, this might help.
					if (retries < MaxTweetDatabaseSendRetries) {
						Task.Factory.StartNew(() => {
							// wait a little and then try again
							var ticksToWait = DateTime.Now.AddSeconds(secondsBetweenSendRetries).Ticks;
							while (ticksToWait > DateTime.Now.Ticks) { /* do nothing */ }
							SaveTweet(tweet, retries + 1);

						});
					} else {
						if (Message != null) {
							Message("Failed to send after " + retries + " tries");
						}
					}
				}
			}
		}

		JObject EncodeTweetToJson(ITweet tweet)
		{
			// encode tweet to json and use SaveToPhpFullJson(string json).
			JObject json = new JObject();

			// add necessary fields to json (synchronize with php script).
			// json.created_at
			json.Add("created_at", tweet.CreatedAt.ToString());
			// json.id_str
			json.Add("id_str", tweet.IdStr);
			// json.in_reply_to_status_id_str
			json.Add("in_reply_to_status_id_str", tweet.InReplyToStatusIdStr);
			// json.in_reply_to_user_id_str
			json.Add("in_reply_to_user_id_str", tweet.InReplyToUserIdStr);
			// json.lang
			// convert tweet.Language to two-letter code like twitter does
			string twoLetterLanguage = Tweetinvi.Core.Extensions.LanguageExtension.GetDescriptionAttribute(tweet.Language);
			json.Add("lang", twoLetterLanguage);
			// json.retweet_count
			json.Add("retweet_count", tweet.RetweetCount.ToString());
			// json.text
			json.Add("text", tweet.Text);
			// json.user
			var jsonUser = new Newtonsoft.Json.Linq.JObject();
			// json.user.screen_name
			jsonUser.Add("screen_name", tweet.Creator.ScreenName);
			// json.user.id_str
			jsonUser.Add("id_str", tweet.Creator.IdStr);
			json.Add("user", jsonUser);

			#region copied 27-02-2015 from php for reference

			//$json = json_decode($_POST['json']);

			//// UTC time when this Tweet was created.
			////$json->created_at
			////$created_at = $json->created_at;
			////$mysqldate = date('y-m-d H:i:s', strtotime($created_at));
			//$mysqldate = date('Y-m-d H:i:s', time());
			////$json->id_str
			//$id_str = $json->id_str;
			//// If the represented Tweet is a reply, this field will contain the integer representation of the original Tweet’s ID.
			////$json->in_reply_to_status_id_str
			//$in_reply_to_status_id_str = $json->in_reply_to_status_id_str;
			////$json->in_reply_to_user_id_str
			//$in_reply_to_user_id_str = $json->in_reply_to_user_id_str;
			////$json->lang
			//$lang = $json->lang;
			////$json->retweet_count
			//$retweet_count = $json->retweet_count;
			//// tweet text
			////$json->text
			//$text = mysqli_real_escape_string($mysqli, $json->text);

			//// object containing info about user
			////$json->user
			//// The screen name, handle, or alias that this user identifies themselves with. screen_names are unique but subject to change. Use id_str as a user identifier whenever possible. Typically a maximum of 15 characters long, but some historical accounts may exist with longer names.
			////	$json->user->screen_name

			//$user_name = mysqli_real_escape_string($mysqli, $json->user->screen_name);

			////	$json->user->id_str
			//$user_id_str = $json->user->id_str;
			#endregion

			return json;
		}


		/// <summary>
		/// sends tweet json string to database.
		/// </summary>
		/// <param name="json"></param>
		void SaveToPhpFullJson(string json)
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
		/// saves directly to database.
		/// </summary>
		/// <param name="message"></param>
		void SaveToDatabase(ITweet tweet)
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


		void SaveToTextFile(ITweet tweet)
		{
			var json = EncodeTweetToJson(tweet);
			SaveToTextFile(json.ToString());
		}


		// write all relevant fields except tweet first (they cannot contain separation chars)
		// format tweet:
		//		replace \n characters with <hhhnewline>
		//		replace encoded chars &lt; with <, &gt; with >, &amp; with &.
		//		replace separator characters with <hhhseparator>
		// write formated tweet at the end of the line
		// after all tweets were saved, import text file in excel, separate based on separation chars (currently ",")
		// replace <hhhnewline> with '\n' in the tweet column
		// replace <hhhseparator> with ',' in the tweet column
		void SaveToTextFile(string json)
		{
			var separationChar = ",";


			StreamWriter sw = new StreamWriter(textFileDatabasePath, true, Encoding.UTF8);
			// save tweet first, and then some other random bullshit
			var j = JObject.Parse(json);

			
			var tweet = j["text"];
			var tweetText = tweet.ToString().Replace("\n", "<hhhnewline>");
			tweetText = tweetText.Replace(",", "<hhhseparator>");
			tweetText = tweetText.Replace("&lt;", "<");
			tweetText = tweetText.Replace("&gt;", ">");
			tweetText = tweetText.Replace("&amp;", "&");

			// add necessary fields to json (synchronized fields with encodejson function)
			// json.created_at
			// json.id_str
			// json.in_reply_to_status_id_str
			// json.in_reply_to_user_id_str
			// json.lang
			// json.retweet_count
			// json.text
			// json.user.screen_name
			// json.user.id_str

			string final = "";
			final += j["created_at"] + separationChar;
			final += j["id_str"] + separationChar;
			final += j["in_reply_to_status_id_str"] + separationChar;
			final += j["in_reply_to_user_id_str"] + separationChar;
			final += j["lang"] + separationChar;
			final += j["retweet_count"] + separationChar;
			final += j["user"]["screen_name"] + separationChar;
			final += j["user"]["id_str"] + separationChar;

			final += tweetText;

			sw.WriteLine(final);
			sw.Close();
		}

	}
}
