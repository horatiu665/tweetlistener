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

		public MainWindow()
		{
			InitializeComponent();

			viewModel = new MainWindowViewModel();

			credentials = new Credentials();
			tweetDatabase = new TweetDatabase();
			keywordDatabase = new KeywordDatabase();
			log = new Log();
			databaseSaver = new DatabaseSaver(log);
			rest = new Rest(databaseSaver, log, tweetDatabase);
			stream = new Stream(databaseSaver, log, rest, keywordDatabase);
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
			checkBox_database.DataContext = databaseSaver;

			// rest log binding
			restView.DataContext = RestMessageList;

			// kData list binding
			keywordListView.ItemsSource = keywordDatabase.KeywordList;

			expansionPanel.DataContext = queryExpansion;

			autoExpansionTimer.Elapsed += (s, a) => {
				AutoExpand();
			};

			restGatherer.TweetFound += (s, t) => {
				App.Current.Dispatcher.Invoke(() => {
					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);
					keywordListView.UpdateLayout();
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

			// saving tweets from Streaming and Rest into TweetDatabase
			stream.stream.MatchingTweetReceived += (s, a) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(a.Tweet, TweetDatabase.TweetData.Sources.Stream, 0, 1));
					keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
				});
				tweetsLastSecond++;
			};
			stream.sampleStream.TweetReceived += (s, a) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(a.Tweet, TweetDatabase.TweetData.Sources.Stream, 0, 1));
					keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
				});
				tweetsLastSecond++;
			};
			restGatherer.TweetFound += (s, t) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
					keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
				});
				tweetsLastSecond++;
			};
			rest.TweetFound += (t) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
					keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
				});
				tweetsLastSecond++;
			};

			tweetsPerSecondTimer.Interval = 1000;
			tweetsPerSecondTimer.Elapsed += (s, a) => {
				tweetsPerSecond = 0.8f * tweetsPerSecond + 0.2f * tweetsLastSecond;
				tweetsLastSecond = 0;
				OnPropertyChanged("TweetsPerSecond");
			};
			tweetsPerSecondTimer.Start();
			tweetsPerSecondLabel.DataContext = this;

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

			// if log
			if (checkBox_Log.IsChecked.Value) {
				log.Start(logPathTextBox.Text, checkBox_databaseMessages.IsChecked.Value);
			}

			autoExpansionTimer.Stop();

			if (autoQueryExpansionCheckbox.IsChecked.Value) {
				var timespan = expansionScheduleTimespan.Value.Value;
				if (timespan == null) {
					log.Output("Timespan null in scheduled expansion field in Streaming Toolbox. Please assign a valid timespan for the scheduler!");
				} else {
					autoExpansionTimer.Interval = timespan.TotalMilliseconds;
				}
				autoExpansionTimer.Start();
			}

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

			if (autoExpansionTimer.Enabled) {
				log.Output("Attempting to stop scheduled expansion");
				autoExpansionTimer.Stop();
			}
		}

		private void dMan_Unloaded(object sender, RoutedEventArgs e)
		{
			// save avalon layout to file
		}

		private void dMan_Loaded(object sender, RoutedEventArgs e)
		{
			// load avalon layout from file

			credentialsPanelLayout.ToggleAutoHide();
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
							tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
						keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
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
							tweetDatabase.AllTweets.Add(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
						keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
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

			App.Current.Dispatcher.Invoke((Action)(() => {
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
			}));
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
			if (restStartDate.Value.HasValue && restEndDate.Value.HasValue) {
				// start this in a new thread
				//Task.Factory.StartNew(() => {
				App.Current.Dispatcher.Invoke(() => {
					rest.TweetsGatheringCycle(restStartDate.Value.Value, restEndDate.Value.Value, restFilterTextBox.Text.Split(',').ToList());
				});
				//});
			} else {
				// highlight startDate and endDate to signal the user that they need to be filled with values. #nicetohave
				log.Output("Problem: Cannot start gathering cycle. Please set a *start date* and an *end date* for the query.");
			}
		}

		private void tweetView_Item_DeleteButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				var content = (TweetDatabase.TweetData)(item.Content);
				var result = MessageBox.Show("Delete tweet " + content.Tweet + " ?", "Delete confirmation", MessageBoxButton.YesNo);

				if (result == MessageBoxResult.Yes) {

					App.Current.Dispatcher.Invoke(() => {
						var hashtagsToUpdate = content.tweet.Hashtags.Select(hEntity => hEntity.Text.ToLower());
						tweetDatabase.AllTweets.Remove(content);
						// update counts of hashtags
						foreach (var k in keywordDatabase.KeywordList) {
							if (hashtagsToUpdate.Contains(k.Keyword.Replace("#", "").ToLower())) {
								k.Count = tweetDatabase.Tweets.Count(td => td.Tweet.ToLower().Contains(k.Keyword.ToLower()));
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

			App.Current.Dispatcher.Invoke(() => {
				tweetDatabase.onlyShowKeywords.Clear();

				// attempts refresh of tweetview
				tweetView.ItemsSource = tweetDatabase.Tweets;

				keywordListView.UnselectAll();
			});
		}

		private void tweetView_DeleteAll(object sender, RoutedEventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to delete all tweets? The database and text files will not be affected.", "Delete confirmation", MessageBoxButton.YesNo);

			if (result == MessageBoxResult.Yes) {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.AllTweets.Clear();
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
			newKeyword = newKeyword.Replace(" ", "");

			if (newKeyword != "") {
				if (!(newKeyword[0] == '#')) {
					newKeyword = newKeyword.Insert(0, "#");
				}
				App.Current.Dispatcher.Invoke(() => {
					keywordDatabase.KeywordList.Add(new KeywordDatabase.KeywordData(newKeyword, 0));

					SetKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);

					keywordAddTextbox.Text = "";
				});
			}
		}

		private void keywordsView_UseAll(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				var newKeys = new ObservableCollection<KeywordDatabase.KeywordData>(keywordDatabase.KeywordList);
				foreach (var kd in newKeys) {
					kd.UseKeyword = true;
				}

				keywordDatabase.KeywordList.Set(newKeys);

				keywordListView.UpdateLayout();
			});
		}
		private void keywordsView_UseNone(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				var newKeys = new ObservableCollection<KeywordDatabase.KeywordData>(keywordDatabase.KeywordList);
				foreach (var kd in newKeys) {
					kd.UseKeyword = false;
				}

				keywordDatabase.KeywordList.Set(newKeys);

				keywordListView.UpdateLayout();
			});
		}

		private void tweetView_ExpandNaive(object sender, RoutedEventArgs e)
		{
			var newKeywords = queryExpansion.ExpandNaive(keywordDatabase.KeywordList.ToList(), tweetDatabase.AllTweets.ToList());
			keywordDatabase.KeywordList.Set(newKeywords);
			keywordDatabase.KeywordList.Update(tweetDatabase.AllTweets);
		}

		private void AutoExpand()
		{
			log.Output("Attempting to expand the query automatically");
			tweetView_ExpandNaive(null, null);
			log.Output("Attempting to restart the stream automatically");
			restartStreamButton_Click(null, null);
		}

		int currentCredentialDefaults = 0;

		private void setCredentialsDefault_Click(object sender, RoutedEventArgs e)
		{
			var creds = (credentials.GetDefaults(currentCredentialDefaults++));
			Access_Token.Text = creds[0];
			Access_Token_Secret.Text = creds[1];
			Consumer_Key.Text = creds[2];
			Consumer_Secret.Text = creds[3];

		}

		private void tweetView_ExpandEfron(object sender, RoutedEventArgs e)
		{
			queryExpansion.ExpandEfron(keywordDatabase.KeywordList, tweetDatabase.AllTweets);

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
		TweetDatabase.TweetList fromFile_newTweets = new TweetDatabase.TweetList();

		bool fromFile_cancelOperation = false;

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

					App.Current.Dispatcher.Invoke(() => {
						foreach (var t in fromFile_newTweets) {
							tweetDatabase.AllTweets.Add(t);
						}
						fromFile_newTweets.Clear();
					});

					if (!fromFile_isLoading) {
						fromFile_updateUiTimer.Stop();
						fromFile_updateUiTimer.Dispose();
					}
				};
				fromFile_updateUiTimer.Start();

				Task.Factory.StartNew(() => {
					var fromTxt = databaseSaver.LoadFromTextFile(databaseSaver.textFileDatabasePath, ref fromFile_percentDone);
					fromFile_Loaded = true;
					log.Output("Loaded files. Dumping them into tweetList");
					var step = 1 / (float)fromTxt.Count;
					fromFile_percentDone = 0;
					foreach (var t in fromTxt) {
						if (fromFile_cancelOperation) {
							fromFile_cancelOperation = false;
							break;
						}
						fromFile_newTweets.Add(t);
						fromFile_percentDone += step;
					}
					fromFile_isLoading = false;

					App.Current.Dispatcher.Invoke(() => {
						((Button)sender).Content = "Load from text file";
					});
					log.Output("Finished loading " + tweetDatabase.AllTweets.Count + " tweets from file");
				});

			} else {
				fromFile_cancelOperation = true;

			}

		}




		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
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
