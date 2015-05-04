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


		public string StreamFilterBinding
		{
			get
			{
				return stream.Filter;
			}
			set
			{
				stream.Filter = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			credentials = new Credentials();
			databaseSaver = new DatabaseSaver();
			log = new Log();
			stream = new Stream(credentials, databaseSaver);
			rest = new Rest(credentials, databaseSaver, log);
			restGatherer = new RestGatherer(rest, log);

			//////// initialize bindings
			// log binding
			logView.DataContext = LogMessageList;
			log.LogOutput += Log_LogOutput;

			// filter binding
			//filterTextbox.TextChanged += (s, a) => { StreamFilterBinding = filterTextbox.Text; };

			// rest log binding
			restView.DataContext = RestMessageList;

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

		private void menu_Help_About_Click(object sender, RoutedEventArgs e)
		{
			var a = new About();
		}

		private void menu_Help_Help_Click(object sender, RoutedEventArgs e)
		{
			var a = new HelpHelper();
		}

		private void startStreamButton_Click(object sender, RoutedEventArgs e)
		{

			stream.Start(stream.Filter);

			loggedIn = true;

			// if log
			if (checkBox_Log.IsChecked.Value) {
				log.Start(logPathTextBox.Text, checkBox_logCounters.IsChecked.Value, checkBox_databaseMessages.IsChecked.Value);
			}

			// if database
			if (checkBox_database.IsChecked.Value) {
				bool connectOnline = databaseConnectionComboBox.SelectedIndex == 0 ? false : true;
				bool saveToDatabaseOrPhp = databaseDestinationComboBox.SelectedIndex == 0 ? false : true;
				databaseSaver.Start(connectOnline, saveToDatabaseOrPhp);

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
			streamingToolboxLayout.ToggleAutoHide();
			logSettingsLayout.ToggleAutoHide();
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
							_restMessageList.Add(new LogMessage(r.Text));

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
							_restMessageList.Add(new LogMessage(r.Text));

						}
					}

				}
			}));

		}

		private bool queryExpanding = false;

		private void restExpansionButton_Click(object sender, RoutedEventArgs e)
		{
			if (!loggedIn) return;

			var button = ((Button)sender);

			if (!queryExpanding) {
				queryExpanding = true;

				button.Content = "Stop Expansion";
				button.IsEnabled = false;

				int expansionCount;
				if (!int.TryParse(restExpansionsMaxExpansionsTextbox.Text, out expansionCount)) {
					expansionCount = 3;
				}

				restGatherer = new RestGatherer(rest, log);

				restExpansionStatusLabel.Content = "Expanding";
				//restExpansionView.DataContext = restGatherer.KeywordList;
				restExpansionListView.DataContext = restGatherer.KeywordList;

				restGatherer.TweetFound += (s, t) => {
					_restMessageList.Add(
						new LogMessage("E " + t.firstExpansion + "; Tweet: " + t.tweet.Text));
					App.Current.Dispatcher.Invoke(() => {
						SetRestExpansionViewKeywordListColumnHeaders(restGatherer, expansionCount);
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
				var filters = restExpansionFilters.Text.Split(',');

				App.Current.Dispatcher.Invoke(() => {
					restGatherer.Algorithm(expansionCount, DateTime.Now.AddDays(-7), DateTime.Now, filters);
				});

				button.IsEnabled = true;

			} else {
				queryExpanding = false;
				button.IsEnabled = false;
				restGatherer.Stop();
				restGatherer.Stopped += (s, a) => {
					App.Current.Dispatcher.Invoke((Action)(() => {
						_restMessageList.Add(
							new LogMessage("RestGatherer successfully stopped"));
						restExpansionStatusLabel.Content = "Stopped";
						button.Content = "Start Expansion";
						button.IsEnabled = true;

					}));
				};

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
			if ((string)(((MenuItem)sender).Header) == "Log in") {
				credentials.TwitterCredentialsInit();
				((MenuItem)sender).Header = "Logged in";
				loggedIn = true;
			}
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

		private void restAddToDatabase_Click(object sender, RoutedEventArgs e)
		{
			databaseSaver.Message += (s) => {
				_restMessageList.Add(new LogMessage(s));

			};
			// add rest data to database.
			rest.AddLastTweetsToDatabase();

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
