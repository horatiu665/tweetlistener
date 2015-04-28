using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Collections.ObjectModel;

namespace WPFTwitter
{	
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindowOld : Window
	{
		private ObservableCollection<LogMessage> _logMessageList = new ObservableCollection<LogMessage>();
		public ObservableCollection<LogMessage> LogMessageList
		{
			get { return _logMessageList; }
		}

		private int _logMessageListMaxLength = 100;
		public string LogMessageListMaxLength
		{
			get { return _logMessageListMaxLength.ToString(); }
			set { 
				int pValue = _logMessageListMaxLength;
				if (int.TryParse(value, out pValue)) {
					_logMessageListMaxLength = pValue;
				} else {
					throw new Exception("Only int values allowed");
				}
			}
		}

		public string StreamFilterBinding
		{
			get
			{
				return Stream.Filter;
			}
			set
			{
				Stream.Filter = value;
			}
		}

		public MainWindowOld()
		{
			
			InitializeComponent();

			logView.DataContext = LogMessageList;
			filterTextBox.TextChanged += (s, a) => { StreamFilterBinding = filterTextBox.Text; };
			Log.LogOutput += Log_LogOutput;
		}

		/// <summary>
		/// called every time the log outputs something. adds that something to logmessagelist. if list too long, keeps latest entries.
		/// </summary>
		/// <param name="message"></param>
		void Log_LogOutput(string message)
		{
			App.Current.Dispatcher.Invoke((Action)(() => {
				_logMessageList.Add(new LogMessage(message));
				while (_logMessageList.Count > _logMessageListMaxLength) {
					_logMessageList.RemoveAt(0);
				}
				logView.ScrollIntoView(logView.Items.GetItemAt(logView.Items.Count-1));
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
			
			Stream.Start(Stream.Filter);
			
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

		private void getRateLimitButton_Click(object sender, RoutedEventArgs e)
		{
			//rest_rateLimitTextBox.Text = Rest.GetRateLimitsString();
		}

		private void initCredentialsButton_Click(object sender, RoutedEventArgs e)
		{
			Stream.TwitterCredentialsInit();
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
