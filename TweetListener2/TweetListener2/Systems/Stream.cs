using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using System.Threading;
using System.IO;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using Tweetinvi.Core.Events.EventArguments;

namespace TweetListener2.Systems
{

    /// <summary>
    /// handles twitter stream connection (and reconnection) and tweet receiving events.
    /// </summary>
    public class Stream : INotifyPropertyChanged
    {
        private Database database;
        private Log log;
        private Rest rest;
        private KeywordDatabase keywordDatabase;
        private TweetDatabase tweetDatabase;
        private Credentials credentials;

        public Database Database
        {
            get
            {
                return database;
            }

            set
            {
                database = value;
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


        /// <summary>
        /// the stream used throughout the program
        /// </summary>
        public Tweetinvi.Core.Interfaces.Streaminvi.IFilteredStream stream;
        public Tweetinvi.Core.Interfaces.Streaminvi.ISampleStream sampleStream;

        /// <summary>
        /// should Stream search using REST when first started? it will use EarliestTweetDate for the SinceDate of the REST cycle - therefore it can be set to another date than my birthday, so that we don't waste a REST cycle every time we restart the stream.
        /// </summary>
        private bool firstRestRecovery = true;
        public bool FirstRestRecovery
        {
            get
            {
                return firstRestRecovery;
            }

            set
            {
                firstRestRecovery = value;
                OnPropertyChanged("FirstRestRecovery");
            }
        }

        private DateTime earliestTweetDate = new DateTime(1993, 7, 23);

        /// <summary>
        /// sincedate for the first REST cycle, when the stream is started for the first time - this is done at startup
        /// </summary>
        public DateTime EarliestTweetDate
        {
            get
            {
                return earliestTweetDate;
            }

            set
            {
                earliestTweetDate = value;
                OnPropertyChanged("EarliestTweetDate");
            }
        }

        /// <summary>
        /// did the stream stop previously? when?
        /// </summary>
        public DateTime RestRecoverySinceDate
        {
            get
            {
                // return the maximum date between the EarliestTweetDate and the TweetDatabase.MostRecentTweetTime - this way, we don't go further back than the largest of those dates. if earliest tweet date is not set, we set it to my birthday just because.
                return DateTime.Compare(EarliestTweetDate, TweetDatabase.MostRecentTweetTime) < 0 ? TweetDatabase.MostRecentTweetTime : EarliestTweetDate;

            }
        }

        /// <summary>
        /// used when intentionally stopping stream, to know not to attempt a restart.
        /// </summary>
        bool intentionalStop = false;

        /// <summary>
        /// true when stream is running.
        /// </summary>
        bool streamRunning = false;
        public bool StreamRunning
        {
            get
            {
                return streamRunning;
            }
            set
            {
                streamRunning = value;
                OnPropertyChanged("StreamRunning");
            }
        }

        /// <summary>
        /// increases every reconnect, and resets to 100 when connection is successful
        /// </summary>
        int reconnectDelayMillis = 1000;

        int tweetsFoundSinceStreamStarted = 0;
        int tweetsDeletedSinceStreamStarted = 0;
        int tweetsDeletedSinceLastLog = 0;

        #region counters bullshit

        /// <summary>
        /// when true, counts events and displays them. when false, does not do those things.
        /// </summary>
        bool countersOn = true;

        public bool CountersOn
        {
            get
            {
                return countersOn;
            }
            set
            {
                countersOn = value;
                Log.Output("Counters " + (value ? "on" : "off"));
            }
        }

        /// <summary>
        /// counts events
        /// </summary>
        List<int> counters;

        /// <summary>
        /// init counters array which counts events from Streaming
        /// </summary>
        void InitCounters()
        {
            if (CountersOn) {
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
        private void CountEvent(int i)
        {
            counters[i]++;

        }

        /// <summary>
        /// returns string of event count, useful for logging and debugging
        /// </summary>
        private string CountersString()
        {
            if (!CountersOn) {
                return "Counters: off";
            }

            string o = "Counters: ";
            for (int i = 0; i < counters.Count; i++) {
                o += (counters[i] + " ");
            }

            return o;
        }

        /// <summary>
        /// sets counter events. SHOULD ONLY BE CALLED ONCE AS EVENTS ARE ANONYMOUS AND CANNOT BE REMOVED WITHOUT REMOVING all other events
        /// </summary>
        void SetCounterEvents()
        {
            #region counters
            stream.DisconnectMessageReceived += (s, a) => { if (countersOn) CountEvent(0); };
            stream.JsonObjectReceived += (s, a) => { if (countersOn) CountEvent(1); };
            stream.LimitReached += (s, a) => { if (countersOn) CountEvent(2); };
            stream.StreamPaused += (s, a) => { if (countersOn) CountEvent(3); };
            stream.StreamResumed += (s, a) => { if (countersOn) CountEvent(4); };
            stream.StreamStarted += (s, a) => { if (countersOn) CountEvent(5); };
            stream.StreamStopped += (s, a) => { if (countersOn) CountEvent(6); };
            stream.TweetDeleted += (s, a) => { if (countersOn) CountEvent(7); };
            stream.TweetLocationInfoRemoved += (s, a) => { if (countersOn) CountEvent(8); };
            //stream.TweetReceived				+= (s, a) => {  if(countersOn) CountEvent(9); }; // for sample stream
            stream.MatchingTweetReceived += (s, a) => { if (countersOn) CountEvent(9); }; // for filtered stream
            stream.TweetWitheld += (s, a) => { if (countersOn) CountEvent(10); };
            stream.UnmanagedEventReceived += (s, a) => { if (countersOn) CountEvent(11); };
            stream.UserWitheld += (s, a) => { if (countersOn) CountEvent(12); };
            stream.WarningFallingBehindDetected += (s, a) => { if (countersOn) CountEvent(13); };

            #endregion

            // bind log to counter events
            #region counters
            stream.DisconnectMessageReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - DisconnectMessageReceived		"); };
            stream.JsonObjectReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - JsonObjectReceived			"); };
            stream.LimitReached
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - LimitReached					"); };
            stream.StreamPaused
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamPaused					"); };
            stream.StreamResumed
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamResumed					"); };
            stream.StreamStarted
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamStarted					"); };
            stream.StreamStopped
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamStopped					"); };
            stream.TweetDeleted
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetDeleted					"); };
            stream.TweetLocationInfoRemoved
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetLocationInfoRemoved 		"); };
            // stream.TweetReceived	
            //	+= (s, a) => { if (countersOn)  log.Output(CountersString() + " - TweetReceived					"); }; // for sample stream
            stream.MatchingTweetReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - MatchingTweetReceived			"); }; // for filtered stream
            stream.TweetWitheld
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetWitheld					"); };
            stream.UnmanagedEventReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - UnmanagedEventReceived		"); };
            stream.UserWitheld
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - UserWitheld					"); };
            stream.WarningFallingBehindDetected
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - WarningFallingBehindDetected	"); };

            #endregion


            #region counters
            sampleStream.DisconnectMessageReceived += (s, a) => { if (countersOn) CountEvent(0); };
            sampleStream.JsonObjectReceived += (s, a) => { if (countersOn) CountEvent(1); };
            sampleStream.LimitReached += (s, a) => { if (countersOn) CountEvent(2); };
            sampleStream.StreamPaused += (s, a) => { if (countersOn) CountEvent(3); };
            sampleStream.StreamResumed += (s, a) => { if (countersOn) CountEvent(4); };
            sampleStream.StreamStarted += (s, a) => { if (countersOn) CountEvent(5); };
            sampleStream.StreamStopped += (s, a) => { if (countersOn) CountEvent(6); };
            sampleStream.TweetDeleted += (s, a) => { if (countersOn) CountEvent(7); };
            sampleStream.TweetLocationInfoRemoved += (s, a) => { if (countersOn) CountEvent(8); };
            sampleStream.TweetReceived += (s, a) => { if (countersOn) CountEvent(9); }; // for sample stream
                                                                                        //sampleStream.MatchingTweetReceived += (s, a) => { if (countersOn) CountEvent(9); }; // for filtered stream
            sampleStream.TweetWitheld += (s, a) => { if (countersOn) CountEvent(10); };
            sampleStream.UnmanagedEventReceived += (s, a) => { if (countersOn) CountEvent(11); };
            sampleStream.UserWitheld += (s, a) => { if (countersOn) CountEvent(12); };
            sampleStream.WarningFallingBehindDetected += (s, a) => { if (countersOn) CountEvent(13); };

            #endregion

            // bind log to counter events
            #region counters
            sampleStream.DisconnectMessageReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - DisconnectMessageReceived		"); };
            sampleStream.JsonObjectReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - JsonObjectReceived			"); };
            sampleStream.LimitReached
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - LimitReached					"); };
            sampleStream.StreamPaused
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamPaused					"); };
            sampleStream.StreamResumed
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamResumed					"); };
            sampleStream.StreamStarted
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamStarted					"); };
            sampleStream.StreamStopped
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - StreamStopped					"); };
            sampleStream.TweetDeleted
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetDeleted					"); };
            sampleStream.TweetLocationInfoRemoved
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetLocationInfoRemoved 		"); };
            sampleStream.TweetReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetReceived					"); }; // for sample stream
                                                                                                                       //sampleStream.MatchingTweetReceived
                                                                                                                       //	+= (s, a) => { if (countersOn)  log.Output(CountersString() + " - MatchingTweetReceived			"); }; // for filtered stream
            sampleStream.TweetWitheld
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - TweetWitheld					"); };
            sampleStream.UnmanagedEventReceived
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - UnmanagedEventReceived		"); };
            sampleStream.UserWitheld
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - UserWitheld					"); };
            sampleStream.WarningFallingBehindDetected
                += (s, a) => { if (countersOn) Log.Output(CountersString() + " - WarningFallingBehindDetected	"); };

            #endregion


        }

        #endregion

        public Stream(Database dbs, Log log, Rest rest, KeywordDatabase keywordDatabase, TweetDatabase tweetDatabase, Credentials credentials)
        {
            this.Database = dbs;
            this.Log = log;
            this.Rest = rest;
            this.KeywordDatabase = keywordDatabase;
            this.TweetDatabase = tweetDatabase;
            this.Credentials = credentials;

            // initialize stuff

            InitCounters();

            // create stream
            sampleStream = Tweetinvi.Stream.CreateSampleStream(); // sample stream = no filter.
            stream = Tweetinvi.Stream.CreateFilteredStream();

            // setup events
            stream.StreamStarted += onStreamStarted;
            stream.StreamStopped += onStreamStopped;
            stream.MatchingTweetReceived += onMatchingTweetReceived;
            stream.JsonObjectReceived += onJsonObjectReceived;
            stream.LimitReached += onLimitReached;
            stream.TweetDeleted += onTweetDeleted;
            sampleStream.StreamStarted += onStreamStarted;
            sampleStream.StreamStopped += onStreamStopped;
            sampleStream.TweetReceived += onTweetReceived;
            sampleStream.JsonObjectReceived += onJsonObjectReceived;
            sampleStream.LimitReached += onLimitReached;
            sampleStream.TweetDeleted += onTweetDeleted;

            // to see wtf is going on. also this is a list of all events possible for the stream. useful at a glance.
            if (CountersOn) {
                SetCounterEvents();
            }

        }

        private void onTweetDeleted(object sender, TweetDeletedEventArgs e)
        {
            tweetsDeletedSinceStreamStarted++;
            tweetsDeletedSinceLastLog++;
        }

        public int GetDeletedTweets()
        {
            var returnValue = tweetsDeletedSinceLastLog;
            tweetsDeletedSinceLastLog = 0;
            return returnValue;
        }

        void onLimitReached(object sender, Tweetinvi.Core.Events.EventArguments.LimitReachedEventArgs e)
        {
            Log.Output("Stream limit reached. Tweets missed: " + e.NumberOfTweetsNotReceived.ToString());

        }

        /// <summary>
        /// start stream
        /// </summary>
        public void Start()
        {
            if (StreamRunning) {
                Log.Output("Stream already running. Not gonna start it again!");
                return;
            }

            if (streamTask != null) streamTask.Dispose();


            stream.ClearTracks();

            AddTracksFromKeywordDatabase();

            if (stream.TracksCount == 0) {
                //log.Output("There are no keywords in the list! Cannot start stream without any keywords, Twitter would slap us. Please add some keywords using the Keywords panel");
                Log.Output("There are no keywords in the list. Attempting to start sample stream (1% of all Tweets ever)");

            }

            StreamStartInsistBetter();

        }

        void AddTracksFromKeywordDatabase()
        {
            foreach (var keyword in KeywordDatabase.KeywordList) {
                if (keyword.UseKeyword) {
                    stream.AddTrack(keyword.Keyword);
                }
            }
        }

        /// <summary>
        /// Starts stream and makes sure it works.
        /// </summary>
        /// <param name="stream"></param>
        private void StreamStartInsistBetter()
        {
            streamTask = Task.Factory.StartNew(StartStreamTask);

            //Console.WriteLine("Finished starting new Task at " + DateTime.Now.ToString());

        }

        /// <summary>
        /// only called from StreamStartInsist().
        /// This function should run in its own thread
        /// </summary>
        private async void StartStreamTask()
        {
            // this needs to be here because Auth.Credentials are actually one set for the whole application, which includes multiple threads per TweetListener. As such, there could not be more than one Stream per TweetListener2 even though each OldWindow appeared to have their own credentials.
            Credentials.SetCurrentCredentialsForThread();

            // restart while stream throws exceptions. (not when it is closed nicely)
            while (true) {

                if (intentionalStop) {
                    intentionalStop = false;
                    break;

                }

                // first wait to not get banned
                await Task.Delay(reconnectDelayMillis);

                // log the start of the stream.
                Log.Output("Stream attempting to start at time " + DateTime.Now.ToString());

                // catch exceptions within this thread. if they happen, we must retry connecting.
                try {
                    if (stream.TracksCount != 0) {
                        await stream.StartStreamMatchingAnyConditionAsync(); // for filtered stream
                    } else {
                        await sampleStream.StartStreamAsync(); // for sample stream
                    }
                    Log.Output("StartStreamAsync() finally over!");
                }
                catch (TaskCanceledException e) {
                    var message = "Task cancelled ";
                    message += (StreamRunning ? "but stream still running" : "and stream is not running");
                    Log.Output(message);

                    // do nothing because task was cancelled. and stop restarting the stream so many damn times
                    return;

                }
                catch (Exception e) {
                    Log.Output("Exception at StartStreamTask() thread: " + e.ToString());

                    //TODO: when exception happens, check if stream is actually running along,
                    // if we should reset stream, or if two streams are attempting to run at the same time.

                }

            }

            StreamRunning = false;

        }

        public void Stop()
        {
            // only call if stream still running but stop has not been called before (intentionalStop is false when not stopping, but true when stop has been called)
            if (StreamRunning && !intentionalStop) {
                if (stream.TracksCount != 0) {
                    //if (stream.StreamState != Tweetinvi.Core.Enum.StreamState.Stop) {
                    stream.StopStream();
                } else {
                    //if (sampleStream.StreamState != Tweetinvi.Core.Enum.StreamState.Stop) {
                    sampleStream.StopStream();
                }
                intentionalStop = true;
            }
        }

        Task restartTask = null;

        public void Restart()
        {
            Stop();
            if (restartTask != null) {
                restartTask.Dispose();
                restartTask = null;
            }
            restartTask = Task.Factory.StartNew(async () => {
                // wait until stream has stopped && streamRunning is false
                Log.Output("Separate thread waiting to start stream after it stops");
                
                while (StreamRunning) {
                    // wait in intervals of 
                    await Task.Delay(reconnectDelayMillis);
                
                }
                Start();
            });
        }

        #region events

        /// <summary>
        /// happens when stream starts. not sure if successfully or also unsuccessfully
        /// </summary>
        private void onStreamStarted(object sender, EventArgs e)
        {
            // log start and print event args
            Log.Output("OnStreamStarted event is called");

            //log.Output("Stream running filter:");
            //log.Output(filter);
            //log.SmallOutput("Stream running filter:");
            //log.SmallOutput(filter);

            StreamRunning = true;

            if (FirstRestRecovery) {
                FirstRestRecovery = false;
                // use Rest for gathering tweets since last time the stream stopped.
                Rest.TweetsGatheringCycle(RestRecoverySinceDate, DateTime.Now, KeywordDatabase.GetUsableKeywords());
            } else {
                Rest.TweetsGatheringCycle(TweetDatabase.MostRecentTweetId, keywordDatabase.GetUsableKeywords());
            }

            streamStartTime = DateTime.Now;
        }

        /// <summary>
        /// called when StreamStopped Tweetinvi event triggers.
        /// </summary>
        private void onStreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
        {
            // wait more next time, to not get banned.
            reconnectDelayMillis *= 2;

            var s = "";
            s += "Stream ran for the duration: " + DateTime.Now.Subtract(streamStartTime);
            s += "\n" + ("Tweets found since stream last started: " + tweetsFoundSinceStreamStarted);
            tweetsFoundSinceStreamStarted = 0;
            s += "\n" + ("Tweets deleted since stream last started (they are still saved): " + tweetsDeletedSinceStreamStarted);
            tweetsDeletedSinceStreamStarted = 0;

            Log.Output(s);

            Log.Output("Stream disconnected.");

            if (e.DisconnectMessage != null) {
                Log.Output("Message code: " + e.DisconnectMessage.Code);
                if (e.DisconnectMessage.Reason != null) {
                    Log.Output("Reason: " + e.DisconnectMessage.Reason);
                }
            }
            if (e.Exception != null) {
                Log.Output("Exception: " + e.Exception.ToString());
            }

        }

        private void onTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.TweetReceivedEventArgs e)
        {
            reconnectDelayMillis = 1000;
            // do this in a separate thread to not kill the stream just because database has errors
            //Task.Factory.StartNew(() => {
            //	databaseSaver.SaveTweet(e.Tweet);

            //});

            tweetsFoundSinceStreamStarted++;
        }

        private void onMatchingTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
        {
            reconnectDelayMillis = 1000;
            // do this in a separate thread to not kill the stream just because database has errors
            //Task.Factory.StartNew(() => {
            //	databaseSaver.SaveTweet(e.Tweet);

            //});

            tweetsFoundSinceStreamStarted++;

        }

        private bool logEveryJson = false;

        public bool LogEveryJson
        {
            get { return logEveryJson; }
            set { logEveryJson = value; }
        }

        private void onJsonObjectReceived(object sender, Tweetinvi.Core.Events.EventArguments.JsonObjectEventArgs e)
        {
            // only save Json when Json is a tweet. Handled in OnMatchingTweetReceived, for now
            //databaseSaver.SaveJson(e.Json);
            JObject json = JObject.Parse(e.Json);

            // spams the log every tweet. not cool. only use for debug purposes
            if (logEveryJson) {
                EvaluateJsonChildren(json);
            }
        }

        /// <summary>
        /// use in EvaluateJsonChildren() to print out the fields which might give useful information in the log
        /// </summary>
        string[] interestingFields = {
            "limit", "disconnect", "warning", "delete",

            "text"
        };
        private Task streamTask;
        private DateTime streamStartTime;

        void EvaluateJsonChildren(JToken j)
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
            Log.Output(s);

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

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
