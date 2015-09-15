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

namespace WPFTwitter.Windows
{

	/// <summary>
	/// Interaction logic for Main2.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		// viewmodel replacement
		Stream stream;
		Rest rest;
		Credentials credentials;
		DatabaseSaver databaseSaver;
		RestGatherer restGatherer;
		Log log;
		KeywordDatabase keywordDatabase;
		TweetDatabase tweetDatabase;
		QueryExpansion queryExpansion;
		PorterStemmerAlgorithm.PorterStemmer porterStemmer;
		MailHelper mailSpammerDisco;
		MailHelperBase mailSpammerConnect;


		MainWindowViewModel viewModel;

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

		public string TweetsPerSecond
		{
			get
			{
				return tweetsPerSecond.ToString("F2") + " tps; " + (tweetsPerSecond * 60).ToString("F0") + " tpm; " + (tweetsPerSecond * 3600).ToString("F0") + " tph";
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
			get { return mailSpammerDisco.emailSpammer; }
			set { mailSpammerDisco.emailSpammer = value; }
		}

		public TimeSpan EmailSpammerTimeSpan
		{
			get { return TimeSpan.FromMilliseconds(mailSpammerDisco.emailTimer.Interval); }
			set { mailSpammerDisco.emailTimer.Interval = value.TotalMilliseconds; }
		}

		public bool EmailSpammerPositive
		{
			get { return mailSpammerConnect.spammingActivated; }
			set { mailSpammerConnect.spammingActivated = value; }
		}

		public TimeSpan EmailSpammerPositiveTimeSpan
		{
			get { return TimeSpan.FromMilliseconds(mailSpammerConnect.emailTimer.Interval); }
			set { mailSpammerConnect.emailTimer.Interval = value.TotalMilliseconds; }
		}

		public string WindowTitle
		{
			get { return this.Title; }
			set
			{
				this.Title = value;
				mailSpammerConnect.windowTitle = value;
			}
		}

		public Dictionary<string, string> commandLineArgs = new Dictionary<string, string>();

		public int DatabaseDestinationComboBoxIndex
		{
			get
			{
				return (int)databaseSaver.saveMethod;
			}
			set
			{
				databaseSaver.saveMethod = (DatabaseSaver.SaveMethods)value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			// based on http://sa.ndeep.me/post/103/how-to-create-smart-wpf-command-line-arguments
			string[] args = Environment.GetCommandLineArgs().Skip(1).ToArray();

			for (int i = 0; i < args.Length; i += 2) {
				string arg = args[i].Replace("/", "");
				commandLineArgs.Add(arg, args[i + 1]);
			}

			// now we can initialize shit from the command line, and we can run batch scripts that open multiple instances of the application with different settings.
			// only thing is to set up those initializations.
			// a list of initializations can be found at the end of this method.

			viewModel = new MainWindowViewModel();

			log = new Log();
			credentials = new Credentials(log);
			keywordDatabase = new KeywordDatabase(log);
			databaseSaver = new DatabaseSaver(log);
			tweetDatabase = new TweetDatabase(databaseSaver);
			rest = new Rest(databaseSaver, log, tweetDatabase);
			stream = new Stream(databaseSaver, log, rest, keywordDatabase, tweetDatabase);
			mailSpammerDisco = new MailHelper(log, stream);
			mailSpammerConnect = new MailHelperBase(log, stream);
			restGatherer = new RestGatherer(rest, log, tweetDatabase, keywordDatabase);
			queryExpansion = new QueryExpansion(log);
			porterStemmer = new PorterStemmerAlgorithm.PorterStemmer();

			//////// initialize bindings
			// log binding
			logPane.DataContext = log;
			logView.DataContext = LogMessageList;
			log.LogOutput += Log_LogOutput;
			checkBox_logCounters.DataContext = stream;
			checkBox_streamJsonSpammer.DataContext = stream;
			checkBox_databaseMessages.DataContext = databaseSaver;
			databaseRetries.DataContext = databaseSaver;
			checkBox_saveToTextFile.DataContext = databaseSaver;
			checkBox_saveToRam.DataContext = tweetDatabase;
			checkBox_database.DataContext = databaseSaver;
			database_textFileDbPathTextBox.DataContext = databaseSaver;
			setCredentialsDefault.DataContext = credentials;
			database_tableNameTextBox.DataContext = databaseSaver;
			startStreamButton.DataContext = stream;
			database_connectionString.DataContext = databaseSaver;

			// rest log binding
			restView.DataContext = RestMessageList;

			// kData list binding
			//keywordListView.ItemsSource = keywordDatabase.KeywordList;
			keywordView.DataContext = keywordDatabase;

			expansionPanel.DataContext = queryExpansion;

			autoExpansionTimer.Elapsed += (s, a) => {
				log.Output("AutoExpansionTimer elapsed");
				AutoExpand();
			};
			// every 10 minutes expand.
			//autoExpansionTimer.Interval = 10*60*1000;

			restGatherer.TweetFound += (s, t) => {
				App.Current.Dispatcher.Invoke(() => {
					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);
					if (keywordDatabase.ContinuousUpdate) {
						keywordListView.UpdateLayout();
					}
				});
			};

			restGatherer.ExpansionFinished += (s, edata) => {
				_restMessageList.Add(
					new LogMessage("E " + edata.expansion + "; Tweets " + edata.TweetCount));

			};
			restGatherer.CycleFinished += (s, sinceDate, untilDate) => {
				_restMessageList.Add(
					new LogMessage("Cycle from " + sinceDate + " until " + untilDate + " finished"));

			};
			restGatherer.Message += (m) => {
				_restMessageList.Add(
					new LogMessage("Message: " + m));

			};
			restGatherer.Stopped += (s, a) => {
				App.Current.Dispatcher.Invoke((Action)(() => {
					_restMessageList.Add(
						new LogMessage("RestGatherer successfully stopped"));

				}));
			};

			// tweet view binding
			tweetView.DataContext = tweetDatabase.Tweets;
			tweetViewContinuousUpdate.DataContext = this;
			tweetViewOnlyEnglish.DataContext = tweetDatabase;
			tweetViewOnlyWithHashtags.DataContext = tweetDatabase;

			// saving tweets from Streaming and Rest into TweetDatabase
			stream.stream.MatchingTweetReceived += (s, a) => {
				Task.Factory.StartNew(() => {
					tweetDatabase.AddTweet(new TweetDatabase.TweetData(a.Tweet, TweetDatabase.TweetData.Sources.Stream, 0, 1));
					updateTweetsNextUpdate = true;

				});
				tweetsLastSecond++;
			};
			stream.sampleStream.TweetReceived += (s, a) => {
				Task.Factory.StartNew(() => {
					tweetDatabase.AddTweet(new TweetDatabase.TweetData(a.Tweet, TweetDatabase.TweetData.Sources.Stream, 0, 1));
					updateTweetsNextUpdate = true;

				});
				tweetsLastSecond++;
			};
			restGatherer.TweetFound += (s, t) => {
				Task.Factory.StartNew(() => {
					tweetDatabase.AddTweet(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
					updateTweetsNextUpdate = true;

				});
				tweetsLastSecond++;
			};
			rest.TweetFound += (t) => {
				Task.Factory.StartNew(() => {
					tweetDatabase.AddTweet(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
					updateTweetsNextUpdate = true;

				});
				tweetsLastSecond++;
			};

			tweetViewUpdateTimer.Interval = 1000;
			tweetViewUpdateTimer.Elapsed += (s, a) => {
				UpdateTweetView();
			};
			tweetViewUpdateTimer.Start();


			tweetsPerSecondTimer.Interval = 1000;
			tweetsPerSecondTimer.Elapsed += (s, a) => {
				tweetsPerSecond = 0.8f * tweetsPerSecond + 0.2f * tweetsLastSecond;
				tweetsLastSecond = 0;
				OnPropertyChanged("TweetsPerSecond");
			};
			tweetsPerSecondTimer.Start();
			tweetsPerSecondLabel.DataContext = this;

			log.Start(logPathTextBox.Text);


			// when credentials change
			credentials.CredentialsChange += (creds) => {
				Access_Token.Text = creds[0];
				Access_Token_Secret.Text = creds[1];
				Consumer_Key.Text = creds[2];
				Consumer_Secret.Text = creds[3];

			};


			// database output messages (not active if !databaseSaver.outputDatabaseMessages)
			databaseSaver.Message += (s) => {
				log.Output(s);
			};




			#region initializations using command line
			if (commandLineArgs.ContainsKey("phpPostPath")) {
				databasePhpPath.Text = commandLineArgs["phpPostPath"];
				databaseSaver.localPhpJsonLink = commandLineArgs["phpPostPath"];
			}
			if (commandLineArgs.ContainsKey("logPath")) {
				logPathTextBox.Text = commandLineArgs["logPath"];
				log.logPath = log.ValidatePath(commandLineArgs["logPath"]);
			}
			if (commandLineArgs.ContainsKey("textFileDbPath")) {
				databaseSaver.TextFileDatabasePath = commandLineArgs["textFileDbPath"];
			}
			if (commandLineArgs.ContainsKey("dbTableName")) {
				databaseSaver.DatabaseTableName = commandLineArgs["dbTableName"];
			}
			if (commandLineArgs.ContainsKey("dbDestination")) {
				databaseSaver.saveMethod = commandLineArgs["dbDestination"] == "php"
					? DatabaseSaver.SaveMethods.PhpPost : DatabaseSaver.SaveMethods.DirectToMysql;
			}
			if (commandLineArgs.ContainsKey("dbConnectionStr")) {
				databaseSaver.ConnectionString = commandLineArgs["dbConnectionStr"];
			}
			if (commandLineArgs.ContainsKey("saveToDatabase")) {
				databaseSaver.SaveToDatabase = commandLineArgs["saveToDatabase"] != "0";
			}
			if (commandLineArgs.ContainsKey("saveToTextFile")) {
				databaseSaver.SaveToTextFileProperty = commandLineArgs["saveToTextFile"] != "0";
			}
			if (commandLineArgs.ContainsKey("saveToRam")) {
				tweetDatabase.SaveToRamProperty = commandLineArgs["saveToRam"] != "0";
			}
			if (commandLineArgs.ContainsKey("outputEventCounters")) {
				stream.CountersOn =
					commandLineArgs["outputEventCounters"] != "0";
			}
			if (commandLineArgs.ContainsKey("outputDatabaseMessages")) {
				databaseSaver.OutputDatabaseMessages =
					commandLineArgs["outputDatabaseMessages"] != "0";
			}
			if (commandLineArgs.ContainsKey("logEveryJson")) {
				stream.LogEveryJson =
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
			if (commandLineArgs.ContainsKey("onlyEnglish")) {
				tweetDatabase.OnlyEnglish =
					commandLineArgs["onlyEnglish"] != "0";
			}
			if (commandLineArgs.ContainsKey("onlyWithHashtags")) {
				tweetDatabase.OnlyWithHashtags =
					commandLineArgs["onlyWithHashtags"] != "0";
			}
			if (commandLineArgs.ContainsKey("credentials")) {
				int credentialsIndex;
				if (int.TryParse(commandLineArgs["credentials"], out credentialsIndex)) {
					currentCredentialDefaults = credentialsIndex;
					credentials.SetCredentials(currentCredentialDefaults);
				}
			}
			if (commandLineArgs.ContainsKey("keywords")) {
				string keywordsString = commandLineArgs["keywords"];
				var keywordsStringList = keywordsString.Split(",".ToCharArray());
				foreach (var ks in keywordsStringList) {
					keywordDatabase.KeywordList.Add(new KeywordDatabase.KeywordData(ks, 0));
				}
			}
			if (commandLineArgs.ContainsKey("windowTitle")) {
				WindowTitle = commandLineArgs["windowTitle"];
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
				if (commandLineArgs.ContainsKey("onlyEnglish")) {
				if (commandLineArgs.ContainsKey("onlyWithHashtags")) {
				if (commandLineArgs.ContainsKey("credentials")) {
				if (commandLineArgs.ContainsKey("keywords")) {
				if (commandLineArgs.ContainsKey("startStream")) {
				... and a few others which I added later and was too lazy to add here too
			 
			*/
			#endregion


		}

		/// <summary>
		/// happens every sometimes, updates tweetView with the latest tweets (if so chosen)
		/// </summary>
		private void UpdateTweetView()
		{
			if (updateTweetsNextUpdate) {
				updateTweetsNextUpdate = false;
				App.Current.Dispatcher.Invoke(() => {
					if (continuousTweetViewUpdate) {
						tweetView.ItemsSource = tweetDatabase.Tweets;
					}
					keywordDatabase.KeywordList.UpdateCount(tweetDatabase.GetAllTweets());
				});
			}
		}

		/// <summary>
		/// called every time the log outputs something. adds that something to logmessagelist. if list too long, keeps latest entries.
		/// </summary>
		/// <param name="message"></param>
		void Log_LogOutput(string message)
		{
			App.Current.Dispatcher.Invoke((Action)(() => {
				_logMessageList.Add(new LogMessage(message));

				/// if scrolling checkbox is activated, scroll into view. else stop scrolling
				if (log.ScrollToLast)
					logView.ScrollIntoView(logView.Items.GetItemAt(logView.Items.Count - 1));
			}));
		}

		private void startStreamButton_Click(object sender, RoutedEventArgs e)
		{

			stream.Start();

			//if (!autoExpansionTimer.Enabled) {
			//	autoExpansionTimer.Start();
			//}

			//if (autoExpansionTimer) {
			//	TimeSpan timespan = expansionScheduleTimespan.Value.HasValue ? expansionScheduleTimespan.Value.Value : TimeSpan.FromHours(1);
			//	if (timespan == null) {
			//		log.Output("Timespan null in scheduled expansion field in Streaming Toolbox. Please assign a valid timespan for the scheduler!");
			//	} else {
			//		autoExpansionTimer.Interval = timespan.TotalMilliseconds;
			//	}
			//	autoExpansionTimer.Start();
			//}

		}

		private void restartStreamButton_Click(object sender, RoutedEventArgs e)
		{
			stopStreamButton_Click(sender, e);

			// wait until stream has stopped && streamRunning is false
			Task.Factory.StartNew(() => {
				log.Output("Separate thread waiting to start stream after it stops");
				while (stream.StreamRunning) {
					// wait
					;
				}
				startStreamButton_Click(sender, e);
			});
		}

		private void stopStreamButton_Click(object sender, RoutedEventArgs e)
		{
			stream.Stop();

			//if (autoExpansionTimer.Enabled) {
			//	log.Output("Attempting to stop scheduled expansion");
			//	autoExpansionTimer.Stop();
			//}
		}

		private void dMan_Unloaded(object sender, RoutedEventArgs e)
		{
			// save avalon layout to file
		}

		private void dMan_Loaded(object sender, RoutedEventArgs e)
		{
			// load avalon layout from file

			//credentialsPanelLayout.ToggleAutoHide();
			//streamingToolboxLayout.ToggleAutoHide();
			//logSettingsLayout.ToggleAutoHide();
		}

		private void restQueryButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.InvokeAsync((Action)(() => {

				string filter = restFilterTextBox.Text;

				bool simpleQuery = ((CheckBox)rest_filter_simpleQuery).IsChecked.Value;
				if (simpleQuery) {

					log.Output("Getting Rest query with filter \"" + filter + "\"");

					// perform Rest query using Rest class
					var res = rest.SearchTweets(filter);

					if (res == null) {
						log.Output(("Not authenticated or invalid credentials"));
						return;

					}

					if (res.Count == 0) {
						log.Output(("No tweets returned"));
					} else {
						foreach (var r in res) {
							tweetDatabase.AddTweet(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
						updateTweetsNextUpdate = true;
					}
				} else {
					var recent = rest_filter_recent.IsChecked.Value;
					var searchParameters = Tweetinvi.Search.CreateTweetSearchParameter(filter);
					searchParameters.SearchType = recent ? Tweetinvi.Core.Enum.SearchResultType.Recent : Tweetinvi.Core.Enum.SearchResultType.Popular;

					// display stuff in restInfoTextBlock.Text = "<here>"
					log.Output(("Getting Rest query with advanced search parameters"));

					var res = rest.SearchTweets(searchParameters);

					if (res == null) {
						log.Output(("Not authenticated or invalid credentials"));
						return;

					}

					if (res.Count == 0) {
						log.Output(("No tweets returned"));
					} else {
						foreach (var r in res) {
							tweetDatabase.AddTweet(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
						updateTweetsNextUpdate = true;
					}

				}
			}));

		}

		private bool queryExpanding = false;



		private void SetTweetViewColumnHeaders()
		{
			// set column headers text to give info about stuff
			var cols = tweetViewGrid.Columns;
			// count languages?
			// count tweets?
			for (int i = 0; i < cols.Count; i++) {
				if (cols[i].Header.ToString().Contains("Tweet")) {
					cols[i].Header = "Tweets (" + tweetDatabase.Tweets.Count + ")";
				}
			}

		}


		private void SetKeywordListColumnHeaders(RestGatherer restGatherer, int expansionCount)
		{
			// set column headers text to give info about stuff
			//var cols = restExpansionView.Columns;
			var cols = restExpansionListViewGrid.Columns;
			cols[0].Header = "Keywords (" + keywordDatabase.KeywordList.Count + ")";
			cols[1].Header = "Count";
			cols[2].Header = "Expansion ";
			for (int i = 0; i <= expansionCount; i++) {
				cols[2].Header += keywordDatabase.KeywordList.Where(kd => kd.Expansion == i).Count().ToString() + " ";
			}
			// set up events for column headers for restExpansionView if they are not set up yet

		}

		private void restRateLimitButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke((Action)(() => {
				// display stuff in restInfoTextBlock.Text = "<here>"
				_restMessageList.Add(new LogMessage("Getting Rate Limit"));

				// gets rate limit using Rest class.
				var rl = rest.GetRateLimits_Search();

				if (rl == null) {
					_restMessageList.Add(new LogMessage("Not authenticated or invalid credentials (GetRateLimits_Search() was null)"));
					return;

				}

				// readable info
				_restMessageList.Add(new LogMessage("Limit: " + rl.Limit));
				_restMessageList.Add(new LogMessage("Remaining: " + rl.Remaining));
				var time = rl.ResetDateTime.Subtract(DateTime.Now);
				_restMessageList.Add(new LogMessage("Reset time: " + time.ToString(@"hh\:mm\:ss")));

			}));
		}


		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.G) {
			//	bool g = panelLogAnchorablePane.CanClose;
			//	bool gg = panelLogAnchorable.CanClose;
			//	var a = 1;
			//}
		}

		private void setCredentialsButton_Click(object sender, RoutedEventArgs e)
		{
			credentials.TwitterCredentialsInit(new List<string>() {
				// "Access_Token"
				Access_Token.Text,
				// "Access_Token_Secret"
				Access_Token_Secret.Text,
				// "Consumer_Key"
				Consumer_Key.Text,
				// "Consumer_Secret"
				Consumer_Secret.Text
			});

		}

		private void databasePhpPath_TextChanged(object sender, TextChangedEventArgs e)
		{
			if (databaseSaver != null) {
				databaseSaver.localPhpJsonLink = ((TextBox)sender).Text;
				_logMessageList.Add(new LogMessage("php path changed to " + databaseSaver.localPhpJsonLink));
			}
		}


		private void keywordListView_Headers_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// order by kData
			if (restGatherer == null) return;
			if (keywordDatabase.KeywordList == null) return;
			if (keywordDatabase.KeywordList.Count == 0) return;
			if (((GridViewColumnHeader)sender).Content == null) return;

			App.Current.Dispatcher.Invoke(() => {
				if (((GridViewColumnHeader)sender).Content.ToString().Contains("Keyword")) {

					keywordDatabase.KeywordList.Set(keywordDatabase.KeywordList.OrderBy(kd => kd.Keyword).ToList());

				} else if (((GridViewColumnHeader)sender).Content.ToString().Contains("Count")) {
					// order by count
					var maxCount = keywordDatabase.KeywordList.Max(kd => kd.Count);
					if (keywordDatabase.KeywordList.FirstOrDefault().Count == maxCount) {
						keywordDatabase.KeywordList.Set(keywordDatabase.KeywordList.OrderBy(kd => kd.Count).ToList());
					} else {
						keywordDatabase.KeywordList.Set(keywordDatabase.KeywordList.OrderByDescending(kd => kd.Count).ToList());
					}
				} else {
					// order by exp
					var maxExp = keywordDatabase.KeywordList.Max(kd => kd.Expansion);
					if (keywordDatabase.KeywordList.FirstOrDefault().Expansion == maxExp) {
						keywordDatabase.KeywordList.Set(keywordDatabase.KeywordList.OrderBy(kd => kd.Expansion).ToList());
					} else {
						keywordDatabase.KeywordList.Set(keywordDatabase.KeywordList.OrderByDescending(kd => kd.Expansion).ToList());
					}
				}
				keywordListView.UpdateLayout();
			});
		}

		private void keywordListView_Item_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (restGatherer == null) return;
			if (keywordDatabase.KeywordList == null) return;
			if (keywordDatabase.KeywordList.Count == 0) return;

			// get clicked row
			var item = ((ListViewItem)sender);
			var content = (KeywordDatabase.KeywordData)(item.Content);

			// get kData
			// in tweet viewer, show only the tweets that contain said kData.
			App.Current.Dispatcher.Invoke((Action)(() => {
				tweetDatabase.onlyShowKeywords.Clear();
				tweetDatabase.onlyShowKeywords.Add(content.Keyword);

				// attempts refresh of tweetview
				tweetView.ItemsSource = tweetDatabase.Tweets;
			}));

		}

		private void tweetView_Headers_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (tweetDatabase.Tweets.Count == 0) return;

			App.Current.Dispatcher.Invoke((Action)(() => {
				var headerName = ((GridViewColumnHeader)sender).Content.ToString();
				List<TweetDatabase.TweetData> newList;
				if (headerName.Contains("Date")) {
					if (tweetDatabase.Tweets.Min(t => t.Date) == tweetDatabase.Tweets.First().Date) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.Date).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.Date).ToList());
					}

				} else if (headerName.Contains("Tweet")) {
					if (tweetDatabase.Tweets.Min(t => t.Tweet) == tweetDatabase.Tweets.First().Tweet) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.Tweet).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.Tweet).ToList());
					}
				} else if (headerName.Contains("UserId")) {
					if (tweetDatabase.Tweets.Min(t => t.UserId) == tweetDatabase.Tweets.First().UserId) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.UserId).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.UserId).ToList());
					}
				} else if (headerName.Contains("Id")) {
					if (tweetDatabase.Tweets.Min(t => t.Id) == tweetDatabase.Tweets.First().Id) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.Id).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.Id).ToList());
					}
				} else if (headerName.Contains("User")) {
					if (tweetDatabase.Tweets.Min(t => t.User) == tweetDatabase.Tweets.First().User) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.User).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.User).ToList());
					}
				} else if (headerName.Contains("Lang")) {
					if (tweetDatabase.Tweets.Min(t => t.Lang) == tweetDatabase.Tweets.First().Lang) {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderByDescending(t => t.Lang).ToList());
					} else {
						newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets.OrderBy(t => t.Lang).ToList());
					}
				} else {
					newList = new List<TweetDatabase.TweetData>(tweetDatabase.Tweets);
				}

				tweetDatabase.Tweets.Clear();
				foreach (var n in newList) {
					tweetDatabase.Tweets.Add(n);

				}

				tweetView.UpdateLayout();
			}));
		}

		private void restExhaustiveQueryButton_Click(object sender, RoutedEventArgs e)
		{
			if (!rest.IsGathering) {
				if (restStartDate.Value.HasValue && restEndDate.Value.HasValue) {
					// start this in a new thread
					//Task.Factory.StartNew(() => {
					App.Current.Dispatcher.Invoke(() => {
						rest.TweetsGatheringCycle(restStartDate.Value.Value, restEndDate.Value.Value, restFilterTextBox.Text.Split(',').ToList());
					});
					//});
					restExhaustiveQueryButton.Content = "Stop Gathering Cycle";
				} else {
					// highlight startDate and endDate to signal the user that they need to be filled with values. #nicetohave
					log.Output("Problem: Cannot start gathering cycle. Please set a *start date* and an *end date* for the query.");
				}
			} else {
				rest.StopGatheringCycle();
			}

			rest.StoppedGatheringCycle -= ResetRestGatheringButton;
			rest.StoppedGatheringCycle += ResetRestGatheringButton;
		}

		private void ResetRestGatheringButton(int tweetCount)
		{
			App.Current.Dispatcher.Invoke(() => {
				restExhaustiveQueryButton.Content = "Start Gathering Cycle";
			});
		}

		private void tweetView_Item_DeleteButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				var tweetToDelete = (TweetDatabase.TweetData)(item.Content);
				var result = MessageBox.Show("Delete tweet " + tweetToDelete.Tweet + " ?", "Delete confirmation", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.Yes) {

					App.Current.Dispatcher.Invoke(() => {
						var hashtagsToUpdate = tweetToDelete.tweet.Hashtags.Select(hEntity => hEntity.Text.ToLower());
						tweetDatabase.RemoveTweet(tweetToDelete);
						// update counts of hashtags
						foreach (var k in keywordDatabase.KeywordList) {
							if (hashtagsToUpdate.Contains(k.Keyword.Replace("#", "").ToLower())) {
								k.Count = tweetDatabase.GetAllTweets().Count(td => td.Tweet.ToLower().Contains(k.Keyword.ToLower()));
							}
						}

						tweetView.ItemsSource = tweetDatabase.Tweets;
					});
				}
			}
		}

		private void keywordView_Item_DeleteButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (KeywordDatabase.KeywordData)(item.Content);
					// delete kData from list
					keywordDatabase.KeywordList.Remove(content);
					// update header counts
					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);
				});
			}
		}

		private void keywordView_Item_StemButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (KeywordDatabase.KeywordData)(item.Content);
					// stem word_i
					var stemmed = porterStemmer.stemTerm(content.Keyword);
					content.Keyword = stemmed;

				});
			}
		}


		private T FindParent<T>(DependencyObject child) where T : DependencyObject
		{
			var parent = VisualTreeHelper.GetParent(child);

			if (parent == null)
				return null;

			if (parent is T)
				return parent as T;
			else
				return FindParent<T>(parent);
		}

		private void tweetView_ResetSelection(object sender, RoutedEventArgs e)
		{
			var message = "";
			if (tweetDatabase.GetAllTweets().Count > 50000) {
				message = "It WILL take forever to load " + tweetDatabase.GetAllTweets().Count + " tweets. Please reconsider or continue at your own risk.";
			} else if (tweetDatabase.GetAllTweets().Count > 10000) {
				message = "It might take quite a while to load " + tweetDatabase.GetAllTweets().Count + " tweets. Please have patience";
			} else {
				message = "";
			}

			MessageBoxResult m = MessageBoxResult.None;
			if (message != "") {
				m = MessageBox.Show("Are you sure you want to reset selection? " + message, "Warning", MessageBoxButton.YesNo);
			}

			if ((message != "" && m == MessageBoxResult.Yes) || message == "") {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.onlyShowKeywords.Clear();

					// attempts refresh of tweetview
					tweetView.ItemsSource = tweetDatabase.Tweets;

					keywordListView.UnselectAll();
				});
			}
		}

		private void tweetView_DeleteAll(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to delete all tweets? The database and text files will not be affected.", "Delete confirmation", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.RemoveAllTweets();
					foreach (var k in keywordDatabase.KeywordList) {
						k.Count = 0;

					}
					keywordListView.ItemsSource = keywordDatabase.KeywordList;
				});
			}
		}

		private void logClearButtonClick(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				_logMessageList.Clear();
			});
			logView.UpdateLayout();
		}

		private void keywordsView_ResetSelection(object sender, RoutedEventArgs e)
		{
			tweetView_ResetSelection(sender, e);
		}

		private void keywordsView_DeleteAll(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to delete all keywords?", "Delete confirmation", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				App.Current.Dispatcher.Invoke(() => {
					keywordDatabase.KeywordList.Clear();

					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);
				});
			}
		}

		private void keywordAddButtonClick(object sender, RoutedEventArgs e)
		{
			var newKeyword = keywordAddTextbox.Text;
			//newKeyword = newKeyword.Replace(" ", "");

			if (newKeyword != "") {
				//if (!(newKeyword[0] == '#')) {
				//	newKeyword = newKeyword.Insert(0, "#");
				//}

				var newKeywordData = new KeywordDatabase.KeywordData(newKeyword, 0);
				newKeywordData.Count = tweetDatabase.GetAllTweets().Count(t => t.ContainsHashtag(newKeyword));

				App.Current.Dispatcher.Invoke(() => {
					keywordDatabase.KeywordList.Add(newKeywordData);

					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);

					keywordAddTextbox.Text = "";
				});
			}
		}

		private void keywordsView_UseAll(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				foreach (var kd in keywordDatabase.KeywordList) {
					kd.UseKeyword = true;
				}

				//keywordDatabase.KeywordList.Set(newKeys);

				keywordListView.UpdateLayout();
			});
		}
		private void keywordsView_UseNone(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				foreach (var kd in keywordDatabase.KeywordList) {
					kd.UseKeyword = false;
				}

				//keywordDatabase.KeywordList.Set(newKeys);

				keywordListView.UpdateLayout();
			});
		}

		private void expansion_ExpandNaive(object sender, RoutedEventArgs e)
		{
			Task.Factory.StartNew(() => {
				var newKeywords = queryExpansion.ExpandNaive(keywordDatabase.KeywordList.ToList(), tweetDatabase.GetAllTweets().ToList());
				//keywordDatabase.KeywordList.UpdateCount(tweetDatabase.tweets);

				// COUNT IS ALREADY PERFORMED IN THE NAIVE EXPANSION!!!

				// update count outside of main thread
				//foreach (var k in newKeywords) {
				//	k.Count = tweetDatabase.tweets.Count(td => td.ContainsHashtag(k.Keyword));
				//}

				if (newKeywords != null && newKeywords.Count > 0) {
					App.Current.Dispatcher.Invoke(() => {
						keywordDatabase.KeywordList.AddRange(newKeywords);
					});
				}
			});
		}

		private void AutoExpand()
		{
			if (stream.StreamRunning) {
				log.Output("Attempting to expand the query automatically");
				Task.Factory.StartNew(() => {
					// before Efron, more tags must be gathered. preferably a full transitive closure
					var newKeys = queryExpansion.ExpandNaive(keywordDatabase.KeywordList.ToList(), tweetDatabase.GetAllTweets());
					if (newKeys != null && newKeys.Count > 0) {
						App.Current.Dispatcher.Invoke(() => {
							// set use to false (we might not want them in the stream query)
							newKeys.ForEach(kd => kd.UseKeyword = false);
							// add the new keywords
							keywordDatabase.KeywordList.AddRange(newKeys);

							// clear language models, every time.
							keywordDatabase.KeywordList.ClearLanguageModels();

							// now we can expand.
							var efronExpanded = queryExpansion.ExpandEfron(keywordDatabase.KeywordList, tweetDatabase.GetAllTweets());

							// we must generate the query from the query model.
							keywordDatabase.KeywordList.Where(kd => efronExpanded.Any(kkk => kkk.Keyword == kd.Keyword)).ToList().ForEach(kd => kd.UseKeyword = true);

							// after expansion, restart stream to apply the new settings.
							log.Output("Attempting to restart the stream automatically");
							restartStreamButton_Click(null, null);
						});
					}
				});
			}
		}

		private void expansion_ExpandEfron(object sender, RoutedEventArgs e)
		{
			if (!queryExpansion.Expanding) {
				Task.Factory.StartNew(() => {
					queryExpansion.ExpandEfron(keywordDatabase.KeywordList, tweetDatabase.GetAllTweets());

					queryExpansion.Expanding = false;
				});
			} else {
				var m = MessageBox.Show("Expansion running. Cancel current expansion?", "wait what?", MessageBoxButton.YesNo);
				if (m == MessageBoxResult.Yes) {
					queryExpansion.Stop();
				}
			}
		}

		int currentCredentialDefaults = 0;

		private void setCredentialsDefault_Click(object sender, RoutedEventArgs e)
		{
			currentCredentialDefaults++;
			credentials.SetCredentials(currentCredentialDefaults);
		}

		bool fromFile_isLoading = false;

		/// <summary>
		/// true when finally loaded from file.
		/// </summary>
		bool fromFile_Loaded = false;

		/// <summary>
		/// 0 to 1, percentage of tweets from file added to tweet list.
		/// </summary>
		float fromFile_percentDone = 0f;
		public float FromFile_percentDone
		{
			get { return fromFile_percentDone; }
			set { fromFile_percentDone = value; }
		}
		float fromFile_tweetCount, fromFile_tweetLoadedCount;

		public string FromFileLoader
		{
			get
			{
				return fromFile_isLoading
							? fromFile_Loaded
								? "Working... percent done: "
									+ fromFile_percentDone.ToString("F3")
								: "Reading lines... percent done: "
									+ fromFile_percentDone.ToString("F3")
							: "Idle"
								;

			}
		}

		System.Timers.Timer fromFile_updateUiTimer;
		//List<TweetDatabase.TweetData> fromFile_newTweets = new List<TweetDatabase.TweetData>();

		bool fromFile_transferingTweets = false;

		int startedTimer, stoppedTimer;

		private void tweetView_LoadFromFile(object sender, RoutedEventArgs e)
		{
			if (!fromFile_isLoading) {

				((Button)sender).Content = "Cancel operation";

				fromFile_Loaded = false;
				fromFile_isLoading = true;
				// start task which updates UI
				fromFile_updateUiTimer = new System.Timers.Timer(100);
				fromFile_updateUiTimer.Elapsed += (s, a) => {
					OnPropertyChanged("FromFileLoader");
					if (fromFile_Loaded) {
						fromFile_percentDone = fromFile_tweetLoadedCount / fromFile_tweetCount;

					}

					if (!fromFile_isLoading) {
						fromFile_updateUiTimer.Stop();
						fromFile_updateUiTimer.Dispose();
					}
				};
				fromFile_updateUiTimer.Start();

				Task.Factory.StartNew(() => {
					var fromTxt = databaseSaver.LoadFromTextFile(databaseSaver.TextFileDatabasePath, ref fromFile_percentDone);
					log.Output("Loaded files. Dumping them into tweetList");
					fromFile_Loaded = true;
					fromFile_tweetCount = fromTxt.Count;
					fromFile_tweetLoadedCount = 0;

					// add in bulk
					if (false) {
						while (fromTxt.Count > 0) {
							var smallList = fromTxt.GetRange(Math.Max(fromTxt.Count - 1000, 0), Math.Min(1000, fromTxt.Count));
							for (int i = 0; i < smallList.Count; i++) {
								tweetDatabase.AddTweet(smallList[i]);
							}
							fromTxt.RemoveRange(Math.Max(fromTxt.Count - 1000, 0), Math.Min(1000, fromTxt.Count));
							fromFile_tweetLoadedCount += 1000;
						}
					}
					// add one by one
					var checkEachTweet = false;
					if (checkEachTweet) {
						for (int i = 0; i < fromTxt.Count; i++) {
							tweetDatabase.AddTweet(fromTxt[i]);
							fromFile_tweetLoadedCount++;
							if (databaseSaver.fromFile_cancelOperation) {
								databaseSaver.fromFile_cancelOperation = false;
								break;
							}
						}
					} else {
						tweetDatabase.AddTweets(fromTxt);
						fromFile_tweetLoadedCount = fromTxt.Count;
					}

					App.Current.Dispatcher.Invoke(() => {
						((Button)sender).Content = "Load from text file";
					});
					log.Output("Finished loading " + fromFile_tweetLoadedCount + " tweets from file");
					fromFile_isLoading = false;

				});

			} else {
				databaseSaver.fromFile_cancelOperation = true;

			}

		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private void keywordsView_Update(object sender, RoutedEventArgs e)
		{
			// counts all keywords
			keywordDatabase.KeywordList.UpdateCount(tweetDatabase.GetAllTweets());

			// update list view
			keywordListView.UpdateLayout();
		}

		private void tweetView_Item_FollowLink(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				var content = (TweetDatabase.TweetData)(item.Content);
				try {
					System.Diagnostics.Process.Start("https://twitter.com/search?q=" + content.Id);
				}
				catch {
					MessageBox.Show("Something went wrong when trying to open link " + "https://twitter.com/search?q=" + content.Id + "\n\n Try again maybe");
				}
			}
		}

		private void tweetView_KeepTweets(object sender, RoutedEventArgs e)
		{
			int howMany;
			if (int.TryParse(tweetView_keepHowManyTweets_textBox.Text, out howMany)) {
				tweetDatabase.KeepTweets(howMany);
				log.Output("Kept only the first " + tweetDatabase.GetAllTweets().Count + " tweets");
				updateTweetsNextUpdate = true;
			} else if (tweetView_keepHowManyTweets_dateBox.Value.HasValue) {
				var date = tweetView_keepHowManyTweets_dateBox.Value.Value;
				tweetDatabase.KeepTweetsBeforeDate(date);
				log.Output("Deleted tweets after date " + date);
				updateTweetsNextUpdate = true;
			}
		}

		private void keywordView_Item_LangModelCalculateButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (KeywordDatabase.KeywordData)(item.Content);
					if (!content.HasLanguageModel) {
						content.LanguageModel = new LanguageModel(content, keywordDatabase.KeywordList, tweetDatabase.GetAllTweets(), null, LanguageModel.SmoothingMethods.BayesianDirichlet, queryExpansion.EfronMu);
					} else {
						log.Output("Keyword " + content.Keyword + " already has a calculated language model");
						log.Output(content.LanguageModel.ToString());
					}
				});
			}
		}

		private void keywordView_Item_LangModelClearButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (KeywordDatabase.KeywordData)(item.Content);
					content.LanguageModel = null;
				});
			}
		}

		private void keywordView_Item_LangModelPrintButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (KeywordDatabase.KeywordData)(item.Content);

					if (content.HasLanguageModel) {
						log.Output(content.LanguageModel.ToString());
					} else {
						log.Output(content.Keyword + " does not have a language model");
					}
				});
			}
		}

		private void tweetView_DeleteSpamTweets(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				tweetDatabase.RemoveAllRT();
				log.Output("Removed tweets starting with RT, now we have " + tweetDatabase.GetAllTweets().Count + " tweets");
			});
		}

		private void keywordsView_DeleteSelected(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to delete selected keywords?", "Delete confirmation", MessageBoxButton.YesNo);
			if (result == MessageBoxResult.Yes) {

			}
		}

		private void keywordsView_ClearLangModels(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				var i = 0;
				while (i < keywordDatabase.KeywordList.Count) {
					keywordDatabase.KeywordList[i].LanguageModel = null;
					i++;
				}

			});
		}

		private void log_forceResetButton(object sender, RoutedEventArgs e)
		{
			log.Restart();
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

	public class LogMessage
	{

		private string _time;

		public string Time
		{
			get { return _time; }
			set { _time = value; }
		}

		private string _message;

		public string Message
		{
			get { return _message; }
			set { _message = value; }
		}

		public LogMessage(string message)
		{
			_time = DateTime.Now.ToString();
			_message = message;
		}

		public override string ToString()
		{
			return _time + " " + _message;
		}

	}
}
