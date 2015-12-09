using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.Interfaces;
using Tweetinvi;
using Tweetinvi.Core.Helpers;
using Tweetinvi.Core.Interfaces.DTO;
using Tweetinvi.Core.Interfaces.Factories;

namespace TweetListener2.Systems
{
    /// <summary>
    /// connects to the twitter events in the Stream and saves tweets to database in various methods.
    /// </summary>
    public class Database
    {
        private Log log;

        public Log Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }


        private Random random;

        public Database(Log l)
        {
            Log = l;
            random = new Random();
        }

        private bool _started = false;

        public bool connectOnline;

        private bool saveToDatabase = true;
        public bool SaveToDatabase
        {
            get
            {
                return saveToDatabase;
            }
            set
            {
                saveToDatabase = value;
            }
        }

        private bool saveToTextFile = true;
        public bool SaveToTextFileProperty
        {
            get { return saveToTextFile; }
            set { saveToTextFile = value; }
        }

        // connection data saved as strings
        private string localConnectionString = @"server=localhost;userid=root;password=;database=twitter";
        public string ConnectionString
        {
            get { return localConnectionString; }
            set { localConnectionString = value; }
        }

        public string localPhpJsonLink = @"http://localhost/tweetlistener/php/saveJson.php";
        public string onlinePhpJsonLink = @"http://hivemindcloud.hostoi.com/saveJson.php";

        private string textFileDatabasePath = "rawJsonBackup.txt";
        public string TextFileDatabasePath
        {
            get { return textFileDatabasePath; }
            set { textFileDatabasePath = value; }
        }

        private string databaseTableName = "json";
        public string DatabaseTableName
        {
            get
            {
                return databaseTableName;
            }
            set
            {
                databaseTableName = value;
            }
        }

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

        private bool outputDatabaseMessages = true;
        public bool OutputDatabaseMessages
        {
            get { return outputDatabaseMessages; }
            set { outputDatabaseMessages = value; }
        }

        public enum SaveMethods
        {
            PhpPost,
            DirectToMysql,
            DirectToSql
        }
        public SaveMethods saveMethod;


        // event triggered every time there is some error that must be logged
        public event DatabaseSaverMessage Message;

        public delegate void DatabaseSaverMessage(string message);


        public void SaveJson(string json)
        {
            if (SaveToDatabase) {
                SaveToPhpFullJson(json);
            }

            if (SaveToTextFileProperty) {
                SaveToTextFile(json);
            }
        }

        /// <summary>
        /// sends tweet to database via chosen method
        /// </summary>
        /// <param name="tweet"></param>
        public void SaveTweet(ITweet tweet, int retries = 0)
        {
            if (SaveToDatabase) {
                // save to php 
                try {
                    if (saveMethod == SaveMethods.DirectToMysql) {
                        // save to mysql
                        Task.Factory.StartNew(() => {
                            SaveToMySQL(tweet, DatabaseTableName);
                        });

                    } else if (saveMethod == SaveMethods.DirectToSql) {
                        // save to mysql
                        Task.Factory.StartNew(() => {
                            SaveToSQL(tweet, DatabaseTableName);
                        });

                    } else if (saveMethod == SaveMethods.PhpPost) {
                        // encode tweet to json and use SaveToPhpFullJson(string json).
                        JObject json = EncodeTweetToJson(tweet);
                        SaveToPhpFullJson(json.ToString());

                    }
                }
                catch (Exception e) {
                    if (e is SqlException) {
                        if (((SqlException)e).Number == 1045) {
                            if (Message != null) {
                                Message("Invalid username or password");
                            }
                        }
                    }
                    if (e is MySqlException) {
                        if (((MySqlException)e).Number == 1045) {
                            if (Message != null) {
                                Message("Invalid username or password");
                            }
                        }
                    }

                    if (Message != null) {
                        if (outputDatabaseMessages)
                            Message(e.ToString());
                    }
                    // retry maxTweetDatabaseSendRetries times to send tweet to database; if error, this might help.
                    if (retries < MaxTweetDatabaseSendRetries) {
                        Task.Factory.StartNew(() => {
                            // wait a little and then try again. wait random amount to spread calls among multiple programs
                            var ticksToWait = DateTime.Now.AddSeconds(secondsBetweenSendRetries + random.NextDouble() * secondsBetweenSendRetries).Ticks;
                            while (ticksToWait > DateTime.Now.Ticks) { /* do nothing */ }
                            SaveTweet(tweet, retries + 1);

                        });
                    } else {
                        if (Message != null) {
                            if (outputDatabaseMessages)
                                Message("Failed to send after " + retries + " tries");
                        }
                    }
                }
            }

            if (SaveToTextFileProperty) {
                SaveToTextFile(tweet);
            }
        }

        public void ResendToDatabase(ITweet tweet, int retries = 0)
        {// save to php 
            try {
                if (saveMethod == SaveMethods.DirectToMysql) {
                    // save to mysql
                    Task.Factory.StartNew(() => {
                        SaveToMySQL(tweet, DatabaseTableName);
                    });

                } else if (saveMethod == SaveMethods.DirectToSql) {
                    // save to mysql
                    Task.Factory.StartNew(() => {
                        SaveToSQL(tweet, DatabaseTableName);
                    });

                } else if (saveMethod == SaveMethods.PhpPost) {
                    // encode tweet to json and use SaveToPhpFullJson(string json).
                    JObject json = EncodeTweetToJson(tweet);
                    SaveToPhpFullJson(json.ToString());

                }
            }
            catch (Exception e) {
                if (e is SqlException) {
                    if (((SqlException)e).Number == 1045) {
                        if (Message != null) {
                            Message("Invalid username or password");
                        }
                    }
                }
                if (e is MySqlException) {
                    if (((MySqlException)e).Number == 1045) {
                        if (Message != null) {
                            Message("Invalid username or password");
                        }
                    }
                }

                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message(e.ToString());
                }
                // retry maxTweetDatabaseSendRetries times to send tweet to database; if error, this might help.
                if (retries < MaxTweetDatabaseSendRetries) {
                    Task.Factory.StartNew(() => {
                        // wait a little and then try again. wait random amount to spread calls among multiple programs
                        var ticksToWait = DateTime.Now.AddSeconds(secondsBetweenSendRetries + random.NextDouble() * secondsBetweenSendRetries).Ticks;
                        while (ticksToWait > DateTime.Now.Ticks) { /* do nothing */ }
                        ResendToDatabase(tweet, retries + 1);

                    });
                } else {
                    if (Message != null) {
                        if (outputDatabaseMessages)
                            Message("Failed to send after " + retries + " tries");
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
            // convert tweet.Language to two-letter code like twitter does - better way would be to add an int and a space, and then the two letter, so that we can read it back in and cast to enum (else I don't know how we can save it back to an enum)
            string twoLetterLanguage = Tweetinvi.Core.Extensions.LanguageExtension.GetLanguageCode(tweet.Language);
            json.Add("lang", twoLetterLanguage);
            // json.retweet_count
            json.Add("retweet_count", tweet.RetweetCount.ToString());
            // json.text
            json.Add("text", tweet.Text);
            // json.user
            var jsonUser = new Newtonsoft.Json.Linq.JObject();
            // json.user.screen_name
            jsonUser.Add("screen_name", tweet.CreatedBy.ScreenName);
            // json.user.id_str
            jsonUser.Add("id_str", tweet.CreatedBy.IdStr);
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
            postData += "&table=" + databaseTableName;
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
                if (outputDatabaseMessages)
                    Message(((HttpWebResponse)response).StatusDescription);
            }
            // Get the stream containing tweetToDelete returned by the server.
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the tweetToDelete.
            string responseFromServer = reader.ReadToEnd();
            // Display the tweetToDelete.
            if (Message != null) {
                if (outputDatabaseMessages)
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
        void SaveToMySQL(ITweet tweet, string tableName)
        {
            string connectionString = ConnectionString;

            try {
                MySqlConnection connection = new MySqlConnection(connectionString);
                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Opening " + connectionString);
                }
                connection.Open();
                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Connection to " + connectionString + " opened successfully");
                }

                string query = "";
                #region setup  query

                string twoLetterLanguage = Tweetinvi.Core.Extensions.LanguageExtension.GetLanguageCode(tweet.Language);

                // the actual query
                query = " INSERT INTO ";
                query += " " + tableName + " ";
                query += " (`id`, `tweet_id_str`, `tweet`, `created_at`, `user_id_str`, `user_name`, `in_reply_to_status_id_str`, `in_reply_to_user_id_str`, `lang`, `retweet_count`) ";
                query += " VALUES ";
                query += @" (NULL, @tweet_id_str, @tweet, @created_at, @user_id_str, @user_name, @in_reply_to_status_id_str, @in_reply_to_user_id_str, @lang, @retweet_count) ";
                query += " ON DUPLICATE KEY UPDATE ";
                query += " retweet_count = @retweet_count";

                #endregion

                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Creating command for " + connectionString);
                }

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@tweet_id_str", tweet.IdStr);
                command.Parameters.AddWithValue("@tweet", tweet.Text);
                command.Parameters.AddWithValue("@created_at", tweet.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                command.Parameters.AddWithValue("@user_id_str", tweet.CreatedBy.IdStr);
                command.Parameters.AddWithValue("@user_name", tweet.CreatedBy.ScreenName);
                command.Parameters.AddWithValue("@in_reply_to_status_id_str", tweet.InReplyToStatusIdStr);
                command.Parameters.AddWithValue("@in_reply_to_user_id_str", tweet.InReplyToUserIdStr);
                command.Parameters.AddWithValue("@lang", twoLetterLanguage);
                command.Parameters.AddWithValue("@retweet_count", tweet.RetweetCount);
                command.CommandType = System.Data.CommandType.Text;
                // timeout in seconds
                command.CommandTimeout = 60;

                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Executing reader for " + connectionString);
                }

                var reader = command.ExecuteReader();

                while (reader.Read()) {
                    foreach (var r in reader) {
                        if (Message != null) {
                            if (outputDatabaseMessages)
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
                throw new Exception("Database MySqlException. Pls retry");
            }
        }

        void SaveToSQL(ITweet tweet, string tableName)
        {
            /// connection string format for SQL ADO.NET taken from Azure:
            /// Server=tcp:tweetlistener.database.windows.net,1433;Database=twitter;User ID=horatiu665@tweetlistener;Password={your_password_here};Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
            string connectionString = ConnectionString;

            SqlConnection connection = new SqlConnection(connectionString);

            if (Message != null) {
                if (outputDatabaseMessages)
                    Message("Opening " + connectionString);
            }
            connection.Open();
            if (Message != null) {
                if (outputDatabaseMessages)
                    Message("Connection to " + connectionString + " opened successfully");
            }

            string query = "";
            #region setup  query

            string twoLetterLanguage = Tweetinvi.Core.Extensions.LanguageExtension.GetLanguageCode(tweet.Language);

            // the blog post here http://samsaffron.com/blog/archive/2007/04/04/14.aspx
            // states that SQL does not support ON DUPLICATE KEY UPDATE
            // and it should rather use something like the below adaptation:
            /*
                begin tran
                   update t with (serializable)
                   set hitCount = hitCount + 1
                   where pk = @id
                   if @@rowcount = 0
                   begin
                      insert t (pk, hitCount)
                      values (@id,1)
                   end
                commit tran

             */

            query += " begin tran					 ";
            query += " 	update " + tableName + " with (serializable) ";
            query += " 	set retweet_count = @retweet_count ";
            query += " 	where tweet_id_str = @tweet_id_str ";
            query += " 	if @@rowcount = 0			 ";
            query += " 	begin						 ";
            query += " 		insert " + tableName + " (id, tweet_id_str, tweet, created_at, user_id_str, user_name, in_reply_to_status_id_str, in_reply_to_user_id_str, lang, retweet_count) ";
            query += " 		values " + @" (NULL, @tweet_id_str, @tweet, @created_at, @user_id_str, @user_name, @in_reply_to_status_id_str, @in_reply_to_user_id_str, @lang, @retweet_count) ";
            query += " 	end							 ";
            query += " commit tran					 ";

            //query = " INSERT INTO ";
            //query += " " + tableName + " ";
            //query += " (`id`, `tweet_id_str`, `tweet`, `created_at`, `user_id_str`, `user_name`, `in_reply_to_status_id_str`, `in_reply_to_user_id_str`, `lang`, `retweet_count`) ";
            //query += " VALUES ";
            //query += @" (NULL, @tweet_id_str, @tweet, @created_at, @user_id_str, @user_name, @in_reply_to_status_id_str, @in_reply_to_user_id_str, @lang, @retweet_count) ";
            //query += " ON DUPLICATE KEY UPDATE ";
            //query += " retweet_count = @retweet_count";

            #endregion


            if (Message != null) {
                if (outputDatabaseMessages)
                    Message("Creating command for " + connectionString);
            }

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@tweet_id_str", tweet.IdStr);
            command.Parameters.AddWithValue("@tweet", tweet.Text);
            command.Parameters.AddWithValue("@created_at", tweet.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("@user_id_str", tweet.CreatedBy.IdStr);
            command.Parameters.AddWithValue("@user_name", tweet.CreatedBy.ScreenName);
            command.Parameters.AddWithValue("@in_reply_to_status_id_str", tweet.InReplyToStatusIdStr);
            command.Parameters.AddWithValue("@in_reply_to_user_id_str", tweet.InReplyToUserIdStr);
            command.Parameters.AddWithValue("@lang", twoLetterLanguage);
            command.Parameters.AddWithValue("@retweet_count", tweet.RetweetCount);
            command.CommandType = System.Data.CommandType.Text;
            // timeout in seconds
            command.CommandTimeout = 60;

            if (Message != null) {
                if (outputDatabaseMessages)
                    Message("Executing reader for " + connectionString);
            }

            var reader = command.ExecuteReader();

            while (reader.Read()) {
                foreach (var r in reader) {
                    if (Message != null) {
                        if (outputDatabaseMessages)
                            Message(r.ToString());
                    }
                }
            }

            connection.Close();


        }

        void SaveToTextFile(ITweet tweet)
        {
            var json = EncodeTweetToJson(tweet);
            json["lang"] = (int)tweet.Language + " " + json["lang"];
            SaveToTextFile(json.ToString());
        }

        /// <summary>
        /// used when saving to / loading from text file.
        /// </summary>
        char separationChar = ',';

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

            StreamWriter sw = new StreamWriter(TextFileDatabasePath, true, Encoding.UTF8);

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

            var separationChar = this.separationChar.ToString();

            string final = "";
            final += j["created_at"] + separationChar;
            final += j["id_str"] + separationChar;
            final += j["in_reply_to_status_id_str"] + separationChar;
            final += j["in_reply_to_user_id_str"] + separationChar;
            // instead of just language, also save int, and cast to enum when reading
            final += j["lang"] + separationChar;

            final += j["retweet_count"] + separationChar;
            final += j["user"]["screen_name"] + separationChar;
            final += j["user"]["id_str"] + separationChar;

            final += tweetText;

            sw.WriteLine(final);
            sw.Close();
        }


        public bool fromFile_cancelOperation = false;


        public List<TweetData> LoadFromTextFile(string path, ref float percentDone)
        {
            List<TweetData> newList = new List<TweetData>();

            var tweetFactory = TweetinviContainer.Resolve<ITweetFactory>();
            if (File.Exists(path)) {
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8)) {
                    float lineCount = File.ReadLines(path).Count();
                    Log.Output("Loading " + lineCount + " lines from file " + path);
                    var linesRead = 0;
                    string line = "";
                    while (sr.Peek() >= 0) {
                        if (fromFile_cancelOperation) {
                            fromFile_cancelOperation = false;
                            break;
                        }

                        line = sr.ReadLine();
                        linesRead++;
                        percentDone = linesRead / lineCount;
                        // parse tweet data from line
                        if (line.Length > 3) {
                            if (line[3] == ',')
                                line = line.Substring(4);

                            var tweetData = line.Split(separationChar);

                            if (tweetData.Length == 9) {
                                //var j = new JObject();
                                //// based on previous function where we save the data, get data in the same order
                                //j["created_at"] = tweetData[0];
                                //j["id_str"] = tweetData[1];
                                //j["in_reply_to_status_id_str"] = tweetData[2];
                                //j["in_reply_to_user_id_str"] = tweetData[3];
                                //j["lang"]  = tweetData[4];
                                //j["retweet_count"] = tweetData[5];
                                //j["user"] = new JObject();
                                //j["user"]["screen_name"] = tweetData[6];
                                //j["user"]["id_str"] = tweetData[7];
                                //j["text"] = tweetData[8];

                                // replace shit in tweet text
                                tweetData[8] = tweetData[8].Replace("<hhhnewline>", "\n");
                                tweetData[8] = tweetData[8].Replace("<hhhseparator>", ",");


                                var tweet = tweetFactory.CreateTweet(tweetData[8]);

                                // date
                                var dateStr = tweetData[0];
                                while (dateStr[0] == ' ') {
                                    dateStr = dateStr.Substring(1);
                                }
                                while (dateStr.Last() == ' ') {
                                    dateStr = dateStr.Substring(0, dateStr.Length - 1);
                                }
                                DateTime date;
                                var didIt = DateTime.TryParse(dateStr, out date);
                                if (!didIt) {
                                    date = DateTime.Now;
                                }

                                Tweetinvi.Core.Enum.Language lang;
                                if (tweetData[4].IndexOf(' ') >= 0) {
                                    lang = (Tweetinvi.Core.Enum.Language)int.Parse(tweetData[4].Substring(0, tweetData[4].IndexOf(' ')));
                                } else {
                                    lang = Tweetinvi.Core.Enum.Language.Undefined;
                                }

                                CustomTweetFormat fakeTweet = new CustomTweetFormat(
                                    date,
                                    tweetData[1],
                                    tweetData[2],
                                    tweetData[3],
                                    lang, // lang
                                    int.Parse(tweetData[5]),
                                    tweetData[6],
                                    tweetData[7],
                                    tweetData[8]
                                    );

                                //var tweet = tweetFactory.GenerateTweetFromJson(j.ToString());


                                newList.Add(new TweetData(fakeTweet, TweetData.Sources.Unknown, 0, 0));
                                //log.Output("just read tweet with id " + fakeTweet.IdStr);
                            }
                        }
                    }

                }
            }

            return newList;
        }

        public List<TweetData> LoadFromDatabase()
        {
            List<TweetData> tweetsFromDatabase = new List<TweetData>();

            try {
                MySqlConnection connection = new MySqlConnection(ConnectionString);
                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Opening " + ConnectionString);
                }
                connection.Open();
                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Connection to " + ConnectionString + " opened successfully");
                }

                string query = "";

                // the actual query
                query = " SELECT * FROM ";
                query += " " + DatabaseTableName + " ";


                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Creating command for " + ConnectionString);
                }

                MySqlCommand command = new MySqlCommand(query, connection);
                command.CommandType = System.Data.CommandType.Text;

                // timeout in seconds
                command.CommandTimeout = 60;

                if (Message != null) {
                    if (outputDatabaseMessages)
                        Message("Executing reader for " + ConnectionString);
                }

                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read()) {
                    if (reader["tweet_id_str"] != null) {
                        // MySQL sent us a huge list of tweets.
                        // Now we read them one by one, and save them to the RAM like we are crazy

                        /*
                        tweet_id_str
                        tweet
                        created_at
                        user_id_str
                        user_name
                        in_reply_to_status_id_str
                        in_reply_to_user_id_str
                        lang
                        retweet_count
                        */

                        int langInt;
                        Tweetinvi.Core.Enum.Language lang;
                        if (int.TryParse(reader["lang"].ToString(), out langInt)) {
                            lang = (Tweetinvi.Core.Enum.Language)langInt;
                        } else {
                            lang = Tweetinvi.Core.Enum.Language.Undefined;
                        }

                        var newTweet = new CustomTweetFormat(
                            (DateTime)reader["created_at"],
                            reader["tweet_id_str"].ToString(),
                            reader["in_reply_to_status_id_str"].ToString(),
                            reader["in_reply_to_user_id_str"].ToString(),
                            lang,
                            (int)(reader["retweet_count"]),
                            reader["user_name"].ToString(),
                            reader["user_id_str"].ToString(),
                            reader["tweet"].ToString()
                            );

                        tweetsFromDatabase.Add(new TweetData(newTweet, TweetData.Sources.Unknown, 0, 0));
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
                throw new Exception("Database MySqlException. Pls retry");
            }

            return tweetsFromDatabase;
        }

    }
}
