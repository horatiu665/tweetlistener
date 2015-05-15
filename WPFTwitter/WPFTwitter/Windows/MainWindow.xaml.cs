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

namespace WPFTwitter.Windows
{

	/// <summary>
	/// Interaction logic for Main2.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		// viewmodel replacement
		Stream stream;
		Rest rest;
		Credentials credentials;
		DatabaseSaver databaseSaver;
		RestGatherer restGatherer;
		Log log;
		TweetDatabase tweetDatabase;
		QueryExpansion queryExpansion;

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


		public MainWindow()
		{
			InitializeComponent();

			viewModel = new MainWindowViewModel();

			credentials = new Credentials();
			tweetDatabase = new TweetDatabase();
			log = new Log();
			databaseSaver = new DatabaseSaver(log);
			rest = new Rest(credentials, databaseSaver, log, tweetDatabase);
			stream = new Stream(credentials, databaseSaver, log, rest);
			restGatherer = new RestGatherer(rest, log, tweetDatabase);
			queryExpansion = new QueryExpansion(log);

			//////// initialize bindings
			// log binding
			logView.DataContext = LogMessageList;
			log.LogOutput += Log_LogOutput;
			checkBox_logCounters.DataContext = stream;
			checkBox_streamJsonSpammer.DataContext = stream;
			databaseRetries.DataContext = databaseSaver;
			checkBox_saveToTextFile.DataContext = databaseSaver;
			checkBox_database.DataContext = databaseSaver;

			// filter binding
			filterTextbox.DataContext = stream;

			// rest log binding
			restView.DataContext = RestMessageList;

			// keyword list binding
			restExpansionListView.DataContext = restGatherer.KeywordList;

			restExpansionsMaxExpansionsTextbox.DataContext = restGatherer.MaxExpansionCount;

			restGatherer.TweetFound += (s, t) => {
				App.Current.Dispatcher.Invoke(() => {
					SetRestExpansionViewKeywordListColumnHeaders(restGatherer, restGatherer.MaxExpansionCount);
					restExpansionListView.UpdateLayout();
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
					restExpansionStatusLabel.Content = "Stopped";
					restExpansionButton.Content = "Start Expansion";
					restExpansionButton.IsEnabled = true;

				}));
			};

			// tweet view binding
			tweetView.DataContext = tweetDatabase.Tweets;

			// saving tweets from Streaming and Rest into TweetDatabase
			stream.stream.MatchingTweetReceived += (s, a) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.Tweets.Add(new TweetDatabase.TweetData(a.Tweet, TweetDatabase.TweetData.Sources.Stream, 0, 1));
				});
			};
			restGatherer.TweetFound += (s, t) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.Tweets.Add(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
				});
			};
			rest.TweetFound += (t) => {
				App.Current.Dispatcher.Invoke(() => {
					tweetDatabase.Tweets.Add(new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, 0, 1));
				});
			};


			// when credentials change
			credentials.CredentialsChange += (creds) => {
				Access_Token.Text = creds[0];
				Access_Token_Secret.Text = creds[1];
				Consumer_Key.Text = creds[2];
				Consumer_Secret.Text = creds[3];

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
				//if(true)
				logView.ScrollIntoView(logView.Items.GetItemAt(logView.Items.Count - 1));
			}));
		}

		private void startStreamButton_Click(object sender, RoutedEventArgs e)
		{

			stream.Start(stream.Filter);


			loggedIn = true;

			// if log
			if (checkBox_Log.IsChecked.Value) {
				log.Start(logPathTextBox.Text, checkBox_databaseMessages.IsChecked.Value);
			}

		}

		private void restartStreamButton_Click(object sender, RoutedEventArgs e)
		{
			stopStreamButton_Click(sender, e);
			startStreamButton_Click(sender, e);
		}

		private void stopStreamButton_Click(object sender, RoutedEventArgs e)
		{
			stream.Stop();
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

			// display stuff in restInfoTextBlock.Text = "<here>"
			_restMessageList.Add(new LogMessage("trying to get rest query"));

			App.Current.Dispatcher.InvokeAsync((Action)(() => {

				string filter = restFilterTextBox.Text;

				bool simpleQuery = ((CheckBox)rest_filter_simpleQuery).IsChecked.Value;
				if (simpleQuery) {

					// display stuff in restInfoTextBlock.Text = "<here>"
					_restMessageList.Add(new LogMessage("Getting Rest query with filter \"" + filter + "\""));

					// perform Rest query using Rest class
					var res = rest.SearchTweets(filter);

					if (res == null) {
						_restMessageList.Add(new LogMessage("Not authenticated or invalid credentials"));
						return;

					}

					if (res.Count == 0) {
						_restMessageList.Add(new LogMessage("No tweets returned"));
					} else {
						foreach (var r in res) {
							tweetDatabase.Tweets.Add(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
					}
				} else {
					var recent = rest_filter_recent.IsChecked.Value;
					var searchParameters = Tweetinvi.Search.CreateTweetSearchParameter(filter);
					searchParameters.SearchType = recent ? Tweetinvi.Core.Enum.SearchResultType.Recent : Tweetinvi.Core.Enum.SearchResultType.Popular;

					// display stuff in restInfoTextBlock.Text = "<here>"
					_restMessageList.Add(new LogMessage("Getting Rest query with advanced search parameters"));

					var res = rest.SearchTweets(searchParameters);

					if (res == null) {
						_restMessageList.Add(new LogMessage("Not authenticated or invalid credentials"));
						return;

					}

					if (res.Count == 0) {
						_restMessageList.Add(new LogMessage("No tweets returned"));
					} else {
						foreach (var r in res) {
							tweetDatabase.Tweets.Add(new TweetDatabase.TweetData(r, TweetDatabase.TweetData.Sources.Rest, 0, 0));
						}
					}

				}
			}));

		}

		private bool queryExpanding = false;

		private void restExpansionButton_Click(object sender, RoutedEventArgs e)
		{
			if (!loggedIn) LogIn_Click(sender, e);

			var button = ((Button)sender);

			if (!queryExpanding) {
				queryExpanding = true;


				button.Content = "Stop Expansion";
				button.IsEnabled = false;

				restGatherer.Reset();

				restExpansionStatusLabel.Content = "Expanding";
				//restExpansionView.DataContext = restGatherer.KeywordList;

				var filters = restExpansionFilters.Text.Split(',');

				App.Current.Dispatcher.Invoke(() => {
					restGatherer.Algorithm(restGatherer.MaxExpansionCount, DateTime.Now.AddDays(-7), DateTime.Now, filters);
				});

				button.IsEnabled = true;

			} else {
				queryExpanding = false;
				button.IsEnabled = false;
				restGatherer.Stop();

			}
		}

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


		private void SetRestExpansionViewKeywordListColumnHeaders(RestGatherer restGatherer, int expansionCount)
		{
			// set column headers text to give info about stuff
			//var cols = restExpansionView.Columns;
			var cols = restExpansionListViewGrid.Columns;
			cols[0].Header = "Keywords (" + restGatherer.KeywordList.Count + ")";
			cols[1].Header = "Count";
			cols[2].Header = "Expansion ";
			for (int i = 0; i <= expansionCount; i++) {
				cols[2].Header += restGatherer.KeywordList.Where(kd => kd.Expansion == i).Count().ToString() + " ";
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

		private void menu_View_Log_Click(object sender, RoutedEventArgs e)
		{
			// activate Log window 

		}

		private void menu_View_LogSettings_Click(object sender, RoutedEventArgs e)
		{
			// activate Log Settings window 
		}

		private void menu_View_StreamingToolbox_Click(object sender, RoutedEventArgs e)
		{
			// activate StreamingToolbox window 
		}

		private void menu_View_Rest_Click(object sender, RoutedEventArgs e)
		{
			// activate Rest window 
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			//if (e.Key == Key.G) {
			//	bool g = panelLogAnchorablePane.CanClose;
			//	bool gg = panelLogAnchorable.CanClose;
			//	var a = 1;
			//}
		}

		public bool loggedIn = false;

		private void LogIn_Click(object sender, RoutedEventArgs e)
		{
			credentials.TwitterCredentialsInit();
			logInButton.Header = "Logged in";
			loggedIn = true;

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


		private void restExpansionListView_Headers_MouseUp(object sender, MouseButtonEventArgs e)
		{
			// order by keyword
			if (restGatherer == null) return;
			if (restGatherer.KeywordList == null) return;
			if (restGatherer.KeywordList.Count == 0) return;
			if (((GridViewColumnHeader)sender).Content == null) return;
			App.Current.Dispatcher.Invoke((Action)(() => {
				if (((GridViewColumnHeader)sender).Content.ToString().Contains("Keyword")) {

					restGatherer.KeywordList.Set(restGatherer.KeywordList.OrderBy(kd => kd.Keyword).ToList());

				} else if (((GridViewColumnHeader)sender).Content.ToString().Contains("Count")) {
					// order by count
					var maxCount = restGatherer.KeywordList.Max(kd => kd.Count);
					if (restGatherer.KeywordList.FirstOrDefault().Count == maxCount) {
						restGatherer.KeywordList.Set(restGatherer.KeywordList.OrderBy(kd => kd.Count).ToList());
					} else {
						restGatherer.KeywordList.Set(restGatherer.KeywordList.OrderByDescending(kd => kd.Count).ToList());
					}
				} else {
					// order by exp
					var maxExp = restGatherer.KeywordList.Max(kd => kd.Expansion);
					if (restGatherer.KeywordList.FirstOrDefault().Expansion == maxExp) {
						restGatherer.KeywordList.Set(restGatherer.KeywordList.OrderBy(kd => kd.Expansion).ToList());
					} else {
						restGatherer.KeywordList.Set(restGatherer.KeywordList.OrderByDescending(kd => kd.Expansion).ToList());
					}
				}
				restExpansionListView.UpdateLayout();
			}));
		}

		private void restExpansionListView_Item_MouseUp(object sender, MouseButtonEventArgs e)
		{
			if (restGatherer == null) return;
			if (restGatherer.KeywordList == null) return;
			if (restGatherer.KeywordList.Count == 0) return;

			// get clicked row
			var item = ((ListViewItem)sender);
			var content = (KeywordDatabase.KeywordData)(item.Content);

			// get keyword
			// in tweet viewer, show only the tweets that contain said keyword.
			App.Current.Dispatcher.Invoke((Action)(() => {
				tweetDatabase.onlyShowKeywords.Clear();
				tweetDatabase.onlyShowKeywords.Add(content.Keyword);

				// attempts refresh of tweetview
				tweetView.ItemsSource = tweetDatabase.Tweets;
			}));

			// somehow the tweetView should update, because the tweetDatabase.Tweets list is different....
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
				App.Current.Dispatcher.Invoke(() => {
					rest.TweetsGatheringCycle(restStartDate.Value.Value, restEndDate.Value.Value, restFilterTextBox.Text.Split(',').ToList());
				});
			} else {
				// highlight startDate and endDate to signal the user that they need to be filled with values. #nicetohave
			}
		}

		private void tweetView_Item_DeleteButton(object sender, RoutedEventArgs e)
		{
			// get clicked row
			var button = ((Button)sender);
			var item = FindParent<ListViewItem>(button);
			if (item != null) {
				App.Current.Dispatcher.Invoke(() => {
					var content = (TweetDatabase.TweetData)(item.Content);
					var hashtagsToUpdate = content.tweet.Hashtags.Select(hEntity => hEntity.Text.ToLower());
					tweetDatabase.Tweets.Remove(content);
					// update counts of hashtags
					foreach (var k in restGatherer.KeywordList) {
						if (hashtagsToUpdate.Contains(k.Keyword.Replace("#", "").ToLower())) {
							k.Count = tweetDatabase.Tweets.Count(td => td.Tweet.ToLower().Contains(k.Keyword.ToLower()));
						}
					}
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

				restExpansionListView.UnselectAll();
			});
		}

		private void tweetView_DeleteAll(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				tweetDatabase.Tweets = new ObservableCollection<TweetDatabase.TweetData>();
				foreach (var k in restGatherer.KeywordList) {
					k.Count = 0;

				}
				// attempts refresh of tweetview
				tweetView.ItemsSource = tweetDatabase.Tweets;
			});
		}


		private void expandStreamButton_Click(object sender, RoutedEventArgs e)
		{
			var oldQuery = stream.Filter.Split(",".ToCharArray()).ToList();
			var newQuery = queryExpansion.Expand(oldQuery, tweetDatabase.Tweets.Select(td => td.tweet).ToList());
			var newQueryString = "";
			foreach (var k in newQuery) {
				newQueryString += k + ", ";
			}
			stream.Filter = newQueryString;

			restartStreamButton_Click(sender, e);
		}

		private void logClearButtonClick(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke(() => {
				_logMessageList.Clear();
			});
			logView.UpdateLayout();
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
