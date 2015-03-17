using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFTwitter.Windows
{
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
			base.Add(item);
			// make sure list is not longer than max length
			while (this.Count > maxLength) {
				this.RemoveAt(0);
			}
		}
	}

	/// <summary>
	/// Interaction logic for Main2.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private LogMessageListCollection _logMessageList = new LogMessageListCollection(10000);
		public LogMessageListCollection LogMessageList
		{
			get { return _logMessageList; }
		}

		private LogMessageListCollection _restMessageList = new LogMessageListCollection(1000);
		public LogMessageListCollection RestMessageList
		{
			get { return _restMessageList; }
		}


		public string StreamFilterBinding
		{
			get
			{
				return Stream.filter;
			}
			set
			{
				Stream.filter = value;
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			//////// initialize bindings
			// log binding
			logView.DataContext = LogMessageList;
			Log.LogOutput += Log_LogOutput;

			// filter binding
			filterTextbox.TextChanged += (s, a) => { StreamFilterBinding = filterTextbox.Text; };

			// rest log binding
			restView.DataContext = RestMessageList;


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

		private void menu_Options_OpenConsole_Click(object sender, RoutedEventArgs e)
		{
			Setup.StartConsole();
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

			Stream.Start(Stream.filter);

			// if log
			if (checkBox_Log.IsChecked.Value) {
				Log.Start(logPathTextBox.Text, checkBox_logCounters.IsChecked.Value, checkBox_databaseMessages.IsChecked.Value);
			}

			// if database
			if (checkBox_database.IsChecked.Value) {
				bool connectOnline = databaseConnectionComboBox.SelectedIndex == 0 ? false : true;
				bool saveToDatabaseOrPhp = databaseDestinationComboBox.SelectedIndex == 0 ? false : true;
				DatabaseSaver.Start(connectOnline, saveToDatabaseOrPhp);

			}
		}

		private void restartStreamButton_Click(object sender, RoutedEventArgs e)
		{
			stopStreamButton_Click(sender, e);
			startStreamButton_Click(sender, e);
		}


		private void stopStreamButton_Click(object sender, RoutedEventArgs e)
		{
			Stream.Stop();
		}

		private void dMan_Unloaded(object sender, RoutedEventArgs e)
		{
			// save avalon layout to file
		}

		private void dMan_Loaded(object sender, RoutedEventArgs e)
		{
			// load avalon layout from file
		}

		private void restQueryButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke((Action)(() => {
				
				string filter = restFilterTextBox.Text;

				bool simpleQuery = false;
				if (simpleQuery) {
					
					// display stuff in restInfoTextBlock.Text = "<here>"
					_restMessageList.Add(new LogMessage("Getting Rest query with filter \"" + filter + "\""));

					// perform Rest query using Rest class
					var res = Rest.SearchTweets(filter);

					if (res == null) {
						_restMessageList.Add(new LogMessage("Not authenticated or invalid credentials"));
						return;

					}

					foreach (var r in res) {
						_restMessageList.Add(new LogMessage(r.Text));

					}
				} else {
					var recent = rest_filter_recent.IsChecked.Value;
					var searchParameters = Tweetinvi.Search.GenerateTweetSearchParameter(filter);
					

				}
			}));
		}

		private void restRateLimitButton_Click(object sender, RoutedEventArgs e)
		{
			App.Current.Dispatcher.Invoke((Action)(() => {
				// display stuff in restInfoTextBlock.Text = "<here>"
				_restMessageList.Add(new LogMessage("Getting Rate Limit"));

				// gets rate limit using Rest class.
				var rl = Rest.GetRateLimits_Search();

				if (rl == null) {
					_restMessageList.Add(new LogMessage("Not authenticated or invalid credentials"));
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

		private void LogIn_Click(object sender, RoutedEventArgs e)
		{
			if ((string)(((MenuItem)sender).Header) == "Log in") {
				Stream.TwitterCredentialsInit();
				((MenuItem)sender).Header = "Logged in";
			}
		}

		private void setCredentialsButton_Click(object sender, RoutedEventArgs e)
		{
			Tweetinvi.TwitterCredentials.ApplicationCredentials = Tweetinvi.TwitterCredentials.CreateCredentials(
				// "Access_Token"
				Access_Token.Text,
				// "Access_Token_Secret"
				Access_Token_Secret.Text,
				// "Consumer_Key"
				Consumer_Key.Text,
				// "Consumer_Secret"
				Consumer_Secret.Text
				);

		}

		private void restAddToDatabase_Click(object sender, RoutedEventArgs e)
		{
			DatabaseSaver.Message += (s) => {
				_restMessageList.Add(new LogMessage(s));

			};
			// add rest data to database.
			Rest.AddLastTweetsToDatabase();
			
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
