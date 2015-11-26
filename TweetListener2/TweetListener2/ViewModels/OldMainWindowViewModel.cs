using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;

using TweetListener2.Systems;
using System.Timers;

namespace TweetListener2.ViewModels
{
    public class OldMainWindowViewModel : ViewModelBase
    {
        public override string Name
        {
            get
            {
                return "OldTweetListener " + WindowTitle;
            }
        }

        public List<string> BatchArgs
        {
            get
            {
                return batchArgs;
            }

            set
            {
                batchArgs = value;
            }
        }

        private List<string> batchArgs;

        public OldMainWindowViewModel() : base()
        {
            batchArgs = new List<string>();
            this.BatchArgs = batchArgs;

            ConstructorTrickster(BatchArgs);
        }

        public OldMainWindowViewModel(List<string> batchArgs) : base()
        {
            this.BatchArgs = batchArgs;

            ConstructorTrickster(BatchArgs);
        }

        /// <summary>
        /// called by the constructors as an initialization (because apparently we cannot have overrides of the constructor
        /// </summary>
        private void ConstructorTrickster(List<string> batchArgs)
        {
            // based on http://sa.ndeep.me/post/103/how-to-create-smart-wpf-command-line-arguments
            //string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

            // get all command line arguments into the args[] array
            string[] args = batchArgs.SelectMany(bArg => {
                if (bArg.Contains(" ")) {
                    return new List<string>() {
                        bArg.Substring(0, bArg.IndexOf(" ")),
                        bArg.Substring(bArg.IndexOf(" "))
                    };
                } else {
                    return new List<string>();
                }
            }).ToArray();

            // clean up the args[] array
            for (int i = 0; i < args.Length; i += 2) {
                string arg = args[i].Replace("/", "");
                var argValue = args[i + 1];
                argValue = argValue.Trim();
                argValue = argValue.Replace("\"", "");
                commandLineArgs.Add(arg, argValue);
            }

            // now we can initialize from the command line, 
            // and we can run batch scripts that open multiple instances of the application with different settings.
            // only thing is to set up those initializations.
            // a list of initializations can be found at the end of this method.

            Log = new Log();
            Credentials = new Credentials(Log);
            KeywordDatabase = new KeywordDatabase(Log);
            DatabaseSaver = new DatabaseSaver(Log);
            TweetDatabase = new TweetDatabase(DatabaseSaver);
            Rest = new Rest(DatabaseSaver, Log, TweetDatabase, Credentials);
            Stream = new Stream(DatabaseSaver, Log, Rest, KeywordDatabase, TweetDatabase, Credentials);
            MailSpammerDisco = new MailHelper(Log, Stream);
            MailSpammerConnect = new MailHelperBase(Log, Stream);
            QueryExpansion = new QueryExpansion(Log);
            PorterStemmer = new PorterStemmer();

            TweetViewUpdateTimer.Interval = 1000;
            TweetViewUpdateTimer.Start();

            AutoExpansionTimer.Elapsed += (s, a) => {
                Log.Output("AutoExpansionTimer elapsed");
                AutoExpand();
            };
            // every 10 minutes expand.
            //autoExpansionTimer.Interval = 10*60*1000;

            // saving tweets from Streaming and Rest into TweetDatabase
            Stream.stream.MatchingTweetReceived += (s, a) => {
                Task.Factory.StartNew(() => {
                    TweetDatabase.AddTweet(new TweetData(a.Tweet, TweetData.Sources.Stream, 0, 1));
                    UpdateTweetsNextUpdate = true;

                });
                TweetsLastSecond++;
            };
            Stream.sampleStream.TweetReceived += (s, a) => {
                Task.Factory.StartNew(() => {
                    TweetDatabase.AddTweet(new TweetData(a.Tweet, TweetData.Sources.Stream, 0, 1));
                    UpdateTweetsNextUpdate = true;

                });
                TweetsLastSecond++;
            };

            Rest.TweetFound += (t) => {
                Task.Factory.StartNew(() => {
                    TweetDatabase.AddTweet(new TweetData(t, TweetData.Sources.Rest, 0, 1));
                    UpdateTweetsNextUpdate = true;

                });
                TweetsLastSecond++;
            };

            // database output messages (not active if !databaseSaver.outputDatabaseMessages)
            DatabaseSaver.Message += (s) => {
                Log.Output(s);
            };

            #region initializations using command line
            if (commandLineArgs.ContainsKey("phpPostPath")) {
                DatabaseSaver.localPhpJsonLink = commandLineArgs["phpPostPath"];
            }
            if (commandLineArgs.ContainsKey("logPath")) {
                Log.Path = (commandLineArgs["logPath"]);
            }
            if (commandLineArgs.ContainsKey("textFileDbPath")) {
                DatabaseSaver.TextFileDatabasePath = commandLineArgs["textFileDbPath"];
            }
            if (commandLineArgs.ContainsKey("dbTableName")) {
                DatabaseSaver.DatabaseTableName = commandLineArgs["dbTableName"];
            }
            if (commandLineArgs.ContainsKey("dbDestination")) {
                DatabaseSaver.saveMethod = commandLineArgs["dbDestination"] == "php"
                    ? DatabaseSaver.SaveMethods.PhpPost : DatabaseSaver.SaveMethods.DirectToMysql;
            }
            if (commandLineArgs.ContainsKey("dbConnectionStr")) {
                DatabaseSaver.ConnectionString = commandLineArgs["dbConnectionStr"];
            }
            if (commandLineArgs.ContainsKey("saveToDatabase")) {
                DatabaseSaver.SaveToDatabase = commandLineArgs["saveToDatabase"] != "0";
            }
            if (commandLineArgs.ContainsKey("saveToTextFile")) {
                DatabaseSaver.SaveToTextFileProperty = commandLineArgs["saveToTextFile"] != "0";
            }
            if (commandLineArgs.ContainsKey("saveToRam")) {
                TweetDatabase.SaveToRamProperty = commandLineArgs["saveToRam"] != "0";
            }
            if (commandLineArgs.ContainsKey("outputEventCounters")) {
                Stream.CountersOn =
                    commandLineArgs["outputEventCounters"] != "0";
            }
            if (commandLineArgs.ContainsKey("outputDatabaseMessages")) {
                DatabaseSaver.OutputDatabaseMessages =
                    commandLineArgs["outputDatabaseMessages"] != "0";
            }
            if (commandLineArgs.ContainsKey("logEveryJson")) {
                Stream.LogEveryJson =
                    commandLineArgs["logEveryJson"] != "0";
            }
            if (commandLineArgs.ContainsKey("emailDisco")) {
                EmailSpammerTimeSpan = TimeSpan.Parse(commandLineArgs["emailDisco"]);
                if (EmailSpammerTimeSpan != TimeSpan.Zero) {
                    EmailSpammer = true;
                }
            }
            if (commandLineArgs.ContainsKey("emailConnected")) {
                EmailSpammerPositiveTimeSpan = TimeSpan.Parse(commandLineArgs["emailConnected"]);
                if (EmailSpammerPositiveTimeSpan != TimeSpan.Zero) {
                    EmailSpammerPositive = true;
                }
            }
            if (commandLineArgs.ContainsKey("restLastDay")) {
                TimeSpan restLastDayTimeSpan;
                if (TimeSpan.TryParse(commandLineArgs["restLastDay"], out restLastDayTimeSpan)) {
                    RestLastDaySchedule = restLastDayTimeSpan;
                    RestLastDay = true;
                } else {
                    RestLastDay = false;
                }
            }
            if (commandLineArgs.ContainsKey("onlyEnglish")) {
                TweetDatabase.OnlyEnglish =
                    commandLineArgs["onlyEnglish"] != "0";
            }
            if (commandLineArgs.ContainsKey("onlyWithHashtags")) {
                TweetDatabase.OnlyWithHashtags =
                    commandLineArgs["onlyWithHashtags"] != "0";
            }
            if (commandLineArgs.ContainsKey("credentials")) {
                int credentialsIndex;
                if (int.TryParse(commandLineArgs["credentials"], out credentialsIndex)) {
                    CurrentCredentialDefaults = credentialsIndex;
                }
            }
            if (commandLineArgs.ContainsKey("keywords")) {
                string keywordsString = commandLineArgs["keywords"];
                var keywordsStringList = keywordsString.Split(",".ToCharArray());
                foreach (var ks in keywordsStringList) {
                    // trim spaces from beginning and end of each keyword
                    var trimmedKeyword = ks.Trim(' ');
                    if (trimmedKeyword.Length > 0) {
                        KeywordDatabase.KeywordList.Add(new KeywordData(trimmedKeyword, 0));
                    }
                }
            }
            if (commandLineArgs.ContainsKey("windowTitle")) {
                WindowTitle = commandLineArgs["windowTitle"];
            }
            // first REST cycle when stream is first started - sometimes it is nice to not use up the rate limits before we can even test stuff, because we might just wanna restart the program after a little crash or whatever.
            if (commandLineArgs.ContainsKey("firstRestSinceDate")) {
                DateTime firstRestSinceDate = stream.EarliestTweetDate;

                if (commandLineArgs["firstRestSinceDate"] == "today") {
                    firstRestSinceDate = DateTime.Today;
                } else if (commandLineArgs["firstRestSinceDate"] == "yesterday") {
                    firstRestSinceDate = DateTime.Today.AddDays(-1);
                } else if (commandLineArgs["firstRestSinceDate"].Contains("days ago")) {
                    int daysAgo;
                    if (int.TryParse(commandLineArgs["firstRestSinceDate"].Split(' ')[0], out daysAgo)) {
                        firstRestSinceDate = DateTime.Today.AddDays(-daysAgo);
                    }
                } else if (DateTime.TryParse(commandLineArgs["firstRestSinceDate"], out firstRestSinceDate)) {
                    // got the value out, do nothing
                }

                stream.EarliestTweetDate = firstRestSinceDate;
            }
            if (commandLineArgs.ContainsKey("startStream")) {
                if (commandLineArgs["startStream"] != "0") {
                    stream.Start();
                }
            }
            // example args list:
            /*
			 start "WPFTwitter" /D "Release - Copy (2)/" "Release - Copy (2)/WPFTwitter.exe" ^
				 /phpPostPath "http://localhost/hhh/tweetlistenerweb/php/saveJson.php" ^
				 /logPath "log.txt" ^
				 /textFileDbPath "rawJsonBackup.txt" ^
				 /dbTableName "gametest1" ^
				 /dbDestination "mysql" ^
				 /dbConnectionStr "server=localhost;userid=root;password=;database=testing" ^
				 /saveToDatabase 1 ^
				 /saveToTextFile 0 ^
				 /saveToRam 0 ^
				 /outputEventCounters 0 ^
				 /outputDatabaseMessages 0 ^
				 /logEveryJson 0 ^
				 /emailDisco 06:00:00 ^
				 /emailConnected 23:59:59 ^
				 /restLastDay 07:30:00 ^ 
                 /onlyEnglish 1 ^
				 /onlyWithHashtags 0 ^
				 /credentials 7 ^
				 /keywords "#callofduty,#cod,#ps4,#pc,#xbox" ^
				 /windowTitle "Game test 1" ^
				 /startStream 0
			 
			because the following if statements are found above
			
				if (commandLineArgs.ContainsKey("phpPostPath")) {
				if (commandLineArgs.ContainsKey("logPath")) {
				if (commandLineArgs.ContainsKey("textFileDbPath")) {
				if (commandLineArgs.ContainsKey("dbTableName")) {
				if (commandLineArgs.ContainsKey("saveToDatabase")) {
				if (commandLineArgs.ContainsKey("saveToTextFile")) {
				if (commandLineArgs.ContainsKey("outputEventCounters")) {
				if (commandLineArgs.ContainsKey("outputDatabaseMessages")) {
				if (commandLineArgs.ContainsKey("logEveryJson")) {
				if (commandLineArgs.ContainsKey("emailDisco")) {
				if (commandLineArgs.ContainsKey("emailConnected")) {
                if (commandLineArgs.ContainsKey("restLastDay")) {
				if (commandLineArgs.ContainsKey("onlyEnglish")) {
				if (commandLineArgs.ContainsKey("onlyWithHashtags")) {
				if (commandLineArgs.ContainsKey("credentials")) {
				if (commandLineArgs.ContainsKey("keywords")) {
				if (commandLineArgs.ContainsKey("startStream")) {
				... and a few others which I added later and was too lazy to add here too
			 
			*/
            #endregion

            RestLastDayTimerSetup();

        }

        void RestLastDayTimerSetup()
        {
            if (restLastDayTimer != null) {
                restLastDayTimer.Stop();
                restLastDayTimer.Close();
            }

            if (RestLastDay) {
                restLastDayTimer = new Timer();

                // set timer interval to the hour being displayed on the GUI
                var now = DateTime.Now;

                // beginning of today plus GUI time, minus current time = difference.
                var timeSpanToNextRest = now.Subtract(new TimeSpan(now.Hour, now.Minute, now.Second)).Add(RestLastDaySchedule).Subtract(now);

                // if date before now, add 24 hours to make it happen tomorrow.
                if (timeSpanToNextRest.CompareTo(TimeSpan.Zero) <= 0)
                    timeSpanToNextRest = timeSpanToNextRest.Add(new TimeSpan(24, 0, 0));

                // make it happen
                restLastDayTimer.Interval = timeSpanToNextRest.TotalMilliseconds;

                restLastDayTimer.Elapsed -= RestLastDayTimer_Elapsed;
                restLastDayTimer.Elapsed += RestLastDayTimer_Elapsed;

                restLastDayTimer.Start();
            }

        }

        private void RestLastDayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // set timer interval to 24 hours, every time it happens. 
            // therefore initial run will be X amount of time, every subsequent run will be 24 hours.
            restLastDayTimer.Interval = TimeSpan.FromHours(24).TotalMilliseconds;

            // perform REST exhaustive gathering for the past 2 days
            rest.TweetsGatheringCycle(DateTime.Now.Subtract(TimeSpan.FromDays(2)), DateTime.Now, KeywordDatabase.GetUsableKeywords());

        }



        #region old main window shitty code copy-paste

        Stream stream;
        Rest rest;
        Credentials credentials;
        DatabaseSaver databaseSaver;
        Log log;
        KeywordDatabase keywordDatabase;
        TweetDatabase tweetDatabase;
        QueryExpansion queryExpansion;
        PorterStemmer porterStemmer;
        MailHelper mailSpammerDisco;
        MailHelperBase mailSpammerConnect;


        public Stream Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        public Rest Rest
        {
            get
            {
                return rest;
            }

            set
            {
                rest = value;
            }
        }

        public Credentials Credentials
        {
            get
            {
                return credentials;
            }

            set
            {
                credentials = value;
            }
        }

        public DatabaseSaver DatabaseSaver
        {
            get
            {
                return databaseSaver;
            }

            set
            {
                databaseSaver = value;
            }
        }

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

        public KeywordDatabase KeywordDatabase
        {
            get
            {
                return keywordDatabase;
            }

            set
            {
                keywordDatabase = value;
            }
        }

        public TweetDatabase TweetDatabase
        {
            get
            {
                return tweetDatabase;
            }

            set
            {
                tweetDatabase = value;
            }
        }

        public QueryExpansion QueryExpansion
        {
            get
            {
                return queryExpansion;
            }

            set
            {
                queryExpansion = value;
            }
        }

        public PorterStemmer PorterStemmer
        {
            get
            {
                return porterStemmer;
            }

            set
            {
                porterStemmer = value;
            }
        }

        public MailHelper MailSpammerDisco
        {
            get
            {
                return mailSpammerDisco;
            }

            set
            {
                mailSpammerDisco = value;
            }
        }

        public MailHelperBase MailSpammerConnect
        {
            get
            {
                return mailSpammerConnect;
            }

            set
            {
                mailSpammerConnect = value;
            }
        }

        public Timer AutoExpansionTimer
        {
            get
            {
                return autoExpansionTimer;
            }

            set
            {
                autoExpansionTimer = value;
            }
        }

        public bool UpdateTweetsNextUpdate
        {
            get
            {
                return updateTweetsNextUpdate;
            }

            set
            {
                updateTweetsNextUpdate = value;
            }
        }

        public int TweetsLastSecond
        {
            get
            {
                return tweetsLastSecond;
            }

            set
            {
                tweetsLastSecond = value;
            }
        }

        public Timer TweetViewUpdateTimer
        {
            get
            {
                return tweetViewUpdateTimer;
            }

            set
            {
                tweetViewUpdateTimer = value;
            }
        }

        public Timer TweetsPerSecondTimer
        {
            get
            {
                return tweetsPerSecondTimer;
            }

            set
            {
                tweetsPerSecondTimer = value;
            }
        }

        public int CurrentCredentialDefaults
        {
            get
            {
                return credentials.CurrentCredentialsIndex;
            }

            set
            {
                credentials.SetCredentials(value);

                OnPropertyChanged(this, new PropertyChangedEventArgs("CurrentCredentialDefaults"));
            }
        }

        public int CredentialsMaxNumber
        {
            get
            {
                return credentials.Count - 1;
            }
        }


        private LogMessageListCollection _logMessageList = new LogMessageListCollection(10000);
        public LogMessageListCollection LogMessageList
        {
            get { return _logMessageList; }
        }

        private LogMessageListCollection _restMessageList = new LogMessageListCollection(2000);
        public LogMessageListCollection RestMessageList
        {
            get { return _restMessageList; }
        }


        private System.Timers.Timer autoExpansionTimer = new System.Timers.Timer();

        private int tweetsLastSecond = 0;
        private float tweetsPerSecond = 0f;
        private System.Timers.Timer tweetsPerSecondTimer = new System.Timers.Timer();

        public float TweetsPerSecond
        {
            get
            {
                return tweetsPerSecond;
            }
            set
            {
                tweetsPerSecond = value;
            }
        }

        private bool continuousTweetViewUpdate = false;
        public bool ContinuousTweetViewUpdate
        {
            get { return continuousTweetViewUpdate; }
            set { continuousTweetViewUpdate = value; }
        }
        bool updateTweetsNextUpdate = false;
        private System.Timers.Timer tweetViewUpdateTimer = new System.Timers.Timer();

        public bool EmailSpammer
        {
            get { return MailSpammerDisco.emailSpammer; }
            set { MailSpammerDisco.emailSpammer = value; }
        }

        public TimeSpan EmailSpammerTimeSpan
        {
            get { return TimeSpan.FromMilliseconds(MailSpammerDisco.emailTimer.Interval); }
            set { MailSpammerDisco.emailTimer.Interval = value.TotalMilliseconds; }
        }

        public bool EmailSpammerPositive
        {
            get { return MailSpammerConnect.spammingActivated; }
            set { MailSpammerConnect.spammingActivated = value; }
        }

        public TimeSpan EmailSpammerPositiveTimeSpan
        {
            get { return TimeSpan.FromMilliseconds(MailSpammerConnect.emailTimer.Interval); }
            set { MailSpammerConnect.emailTimer.Interval = value.TotalMilliseconds; }
        }

        public string WindowTitle
        {
            get { return MailSpammerConnect.windowTitle; }
            set
            {
                MailSpammerConnect.windowTitle = value;
                OnPropertyChanged(this, new PropertyChangedEventArgs("Name"));
            }
        }

        public Dictionary<string, string> commandLineArgs = new Dictionary<string, string>();

        public int DatabaseDestinationComboBoxIndex
        {
            get
            {
                return (int)DatabaseSaver.saveMethod;
            }
            set
            {
                DatabaseSaver.saveMethod = (DatabaseSaver.SaveMethods)value;
            }
        }


        /// <summary>
        /// called every time the log outputs something. adds that something to logmessagelist. if list too long, keeps latest entries.
        /// </summary>
        /// <param name="message"></param>
        public void Log_LogOutput(string message)
        {
            App.Current.Dispatcher.Invoke((Action)(() => {
                _logMessageList.Add(new LogMessage(message));

                /// if scrolling checkbox is activated, scroll into view. else stop scrolling
                //if (log.ScrollToLast)
                //    logView.ScrollIntoView(logView.Items.GetItemAt(logView.Items.Count - 1));
            }));
        }

        bool fromFile_isLoading = false;

        /// <summary>
        /// true when finally loaded from file.
        /// </summary>
        bool fromFile_Loaded = false;


        public bool FromFile_isLoading
        {
            get
            {
                return fromFile_isLoading;
            }

            set
            {
                fromFile_isLoading = value;
            }
        }

        public bool FromFile_Loaded
        {
            get
            {
                return fromFile_Loaded;
            }

            set
            {
                fromFile_Loaded = value;
            }
        }

        /// <summary>
        /// 0 to 1, percentage of tweets from file added to tweet list.
        /// </summary>
        float fromFile_percentDone = 0f;

        public float FromFile_percentDone
        {
            get
            {
                return fromFile_percentDone;
            }

            set
            {
                fromFile_percentDone = value;
            }
        }

        public string FromFileLoader
        {
            get
            {
                return FromFile_isLoading
                            ? FromFile_Loaded
                                ? "Working... percent done: "
                                    + FromFile_percentDone.ToString("F3")
                                : "Reading lines... percent done: "
                                    + FromFile_percentDone.ToString("F3")
                            : "Idle"
                                ;

            }
        }

        public string TweetsPerSecondString
        {
            get
            {
                return TweetsPerSecond.ToString("F2") + " tps; " + (TweetsPerSecond * 60).ToString("F0") + " tpm; " + (TweetsPerSecond * 3600).ToString("F0") + " tph";
            }
        }

        public Timer FromFile_updateUiTimer
        {
            get
            {
                return fromFile_updateUiTimer;
            }

            set
            {
                fromFile_updateUiTimer = value;
            }
        }

        public float FromFile_tweetCount
        {
            get
            {
                return fromFile_tweetCount;
            }

            set
            {
                fromFile_tweetCount = value;
            }
        }

        public float FromFile_tweetLoadedCount
        {
            get
            {
                return fromFile_tweetLoadedCount;
            }

            set
            {
                fromFile_tweetLoadedCount = value;
            }
        }

        float fromFile_tweetCount, fromFile_tweetLoadedCount;

        System.Timers.Timer fromFile_updateUiTimer;
        //List<TweetData> fromFile_newTweets = new List<TweetData>();

        bool fromFile_transferingTweets = false;

        int startedTimer, stoppedTimer;


        public void AutoExpand()
        {
            Log.Output("AutoExpand not implemented. Too risky!");
            return;
            if (Stream.StreamRunning) {
                Log.Output("Attempting to expand the query automatically");
                Task.Factory.StartNew(() => {
                    // before Efron, more tags must be gathered. preferably a full transitive closure
                    var newKeys = QueryExpansion.ExpandNaive(KeywordDatabase.KeywordList.ToList(), TweetDatabase.GetAllTweets());
                    if (newKeys != null && newKeys.Count > 0) {
                        App.Current.Dispatcher.Invoke(() => {
                            // set use to false (we might not want them in the stream query)
                            newKeys.ForEach(kd => kd.UseKeyword = false);
                            // add the new keywords
                            KeywordDatabase.KeywordList.AddRange(newKeys);

                            // clear language models, every time.
                            KeywordDatabase.KeywordList.ClearLanguageModels();

                            // now we can expand.
                            var efronExpanded = QueryExpansion.ExpandEfron(KeywordDatabase.KeywordList, TweetDatabase.GetAllTweets());

                            // we must generate the query from the query model.
                            KeywordDatabase.KeywordList.Where(kd => efronExpanded.Any(kkk => kkk.Keyword == kd.Keyword)).ToList().ForEach(kd => kd.UseKeyword = true);

                            // after expansion, restart stream to apply the new settings.
                            Log.Output("Attempting to restart the stream automatically");
                            Stream.Restart();

                        });
                    }
                });
            }
        }


        #endregion old main window shitty code copy-paste

        internal void ResendToDatabase(object sender, RoutedEventArgs e)
        {
            tweetDatabase.ResendAllToDatabase();

        }

        /// <summary>
        /// if true, every day at X hours it will run an exhaustive REST gathering thing for last 2 days.
        /// </summary>
        private bool restLastDay = true;
        public bool RestLastDay
        {
            get
            {
                return restLastDay;
            }

            set
            {
                restLastDay = value;
                if (restLastDay) {
                    RestLastDayTimerSetup();
                }
            }
        }

        private TimeSpan restLastDaySchedule = new TimeSpan(7, 30, 0);
        public TimeSpan RestLastDaySchedule
        {
            get
            {
                return restLastDaySchedule;
            }

            set
            {
                restLastDaySchedule = value;
            }
        }

        private Timer restLastDayTimer;

        DateTime streamDeletedTweetsButton_LastPressDate = new DateTime(1993, 7, 23);
        internal void streamDeletedTweetsButton(object sender, RoutedEventArgs e)
        {
            var s = stream.GetDeletedTweets() + " tweets deleted since you last pressed the button, on " + streamDeletedTweetsButton_LastPressDate;
            streamDeletedTweetsButton_LastPressDate = DateTime.Now;
            Log.Output(s);
        }

        internal void tweetView_Count(object sender, RoutedEventArgs e)
        {
            var s = "";
            s += ("Counting Tweets...");
            s += "\n" +(tweetDatabase.GetAllTweets().Count + " tweets in the RAM");
            s += "\n" +(tweetDatabase.Tweets.Count + " tweets shown in the tweetView panel");
            s += "\n" +(tweetDatabase.GetAllTweets().Count(td => td.source == TweetData.Sources.Stream) + " stream tweets");
            s += "\n" +(tweetDatabase.GetAllTweets().Count(td => td.source == TweetData.Sources.Rest) + " rest tweets");
            s += "\n" +(tweetDatabase.GetAllTweets().Count(td => td.source == TweetData.Sources.Unknown) + " unknown tweets");
            s += "\n" +("Top languages: ");
            var langGroups = tweetDatabase.GetAllTweets().GroupBy(td => td.Lang).OrderByDescending(g => g.Count());
            for (int i = 0; i < Math.Min(langGroups.Count(), 5); i++) {
                s += "\n" +(langGroups.ElementAt(i).Key + " - " + langGroups.ElementAt(i).Count());
            }
            s += "\n" +(tweetDatabase.GetAllTweets().Count(td => td.Date.CompareTo(DateTime.Today) >= 0) + " tweets since " + DateTime.Today);
            s += "\n" +(tweetDatabase.GetAllTweets().Count(td => td.Date.CompareTo(DateTime.Today.AddDays(-1)) >= 0 && td.Date.CompareTo(DateTime.Today) < 0) + " tweets since " + DateTime.Today.AddDays(-1) + " until " + DateTime.Today);
            s += "\n" +("End of counting tweets");

            Log.Output(s);
        }
    }


    public class LogMessageListCollection : ObservableCollection<LogMessage>
    {
        public LogMessageListCollection(int maxLength = 1000)
        {
            this.maxLength = maxLength;
        }

        /// <summary>
        /// list does not exceed this length. when it does, first elements are removed until length reached.
        /// </summary>
        public int maxLength = 1000;

        /// <summary>
        /// adds elements at end while keeping length limit by removing the ones at the start of the list
        /// </summary>
        /// <param name="item"></param>
        new public void Add(LogMessage item)
        {
            App.Current.Dispatcher.Invoke((Action)(() => {

                base.Add(item);
                // make sure list is not longer than max length
                while (this.Count > maxLength) {
                    this.RemoveAt(0);
                }
            }));
        }
    }

    public class DatabaseSaver : Database
    {
        public DatabaseSaver(Log l) : base(l)
        {
            base.Log = l;
        }
    }
}
