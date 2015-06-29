using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;
using Newtonsoft.Json.Linq;

namespace WPFTwitter
{
	/// <summary>
	/// handles listening to events and printing out logs to the console and to log files.
	/// </summary>
	public class Log
	{

		public Log()
		{

		}

		private bool _started = false;

		public bool Started
		{
			get { return _started; }
		}

		StreamWriter logWriter;
		public string logPath = "log.txt";
		public double restartLogIntervalMillis = 30000;
		System.Timers.Timer logRestartTimer;

		StreamWriter smallLogWriter;
		private string smallLogPath { get { return "small" + logPath; } }
		public double restartSmallLogIntervalMillis = 30037;
		System.Timers.Timer smallLogRestartTimer;

		public delegate void LogOutputEventHandler(string message);
		public event LogOutputEventHandler LogOutput, SmallLogOutput;

		public bool ScrollToLast
		{
			get;
			set;
		}

		/// <summary>
		/// returns a valid version of the path given
		/// </summary>
		/// <param name="path">path to validate</param>
		/// <returns>valid path that can be used</returns>
		public string ValidatePath(string path)
		{
			try {
				string fullPath = Path.GetFullPath(path);
				// if no error, path is valid
				if (path.EndsWith(".txt")) {
					return path;
				} else {
					return path + ".txt";
				}
			}
			catch {
				Output("Path invalid. Using \"log.txt\". Try not to use fancy characters. Just letters and numbers");
				return "log.txt";
			}

		}

		/// <summary>
		/// Log module starts here
		/// </summary>
		public void Start(string path)
		{
			if (!_started) {

				logPath = ValidatePath(path);

				// open log writer
				logWriter = new StreamWriter(logPath, true);
				Output("New log started");
				
				// small log init
				smallLogWriter = new StreamWriter(smallLogPath, true);
				SmallOutput("New log started");
				
				// once in a while stop and start log, so we do not lose all the data in the log.
				logRestartTimer = new System.Timers.Timer(restartLogIntervalMillis);
				logRestartTimer.Start();
				logRestartTimer.Elapsed += (s, a) => {
					if (logWriter != null && !writing) {
						logWriter.Close();
						logWriter = new StreamWriter(logPath, true);
					} else {
						logRestartTimer.Stop();
					}
				};

				// same thing for small log
				smallLogRestartTimer = new System.Timers.Timer(restartSmallLogIntervalMillis);
				smallLogRestartTimer.Start();
				smallLogRestartTimer.Elapsed += (s, a) => {
					if (smallLogWriter != null) {
						smallLogWriter.Close();
						smallLogWriter = new StreamWriter(smallLogPath, true);
					} else {
						smallLogRestartTimer.Stop();
					}
				};

				// bind to any exception
				//ExceptionHandler.WebExceptionReceived += (s, a) => {
				//	var exception = (Tweetinvi.Core.Exceptions.ITwitterException)a.Value;
				//	var statusCode = exception.StatusCode;
				//	if (exception != null && statusCode != null) {
				//		Output("Web exception received. Status code " + statusCode + "\n" + exception.ToString());
				//	}
				//};

				StartSafeOutput();

				_started = true;
			}

		}

		public void Stop()
		{
			// too dangerous to stop log, because events are not cleaned up, and restarting it would mean double the prints.

			//logRestartTimer.Stop();
			//logWriter.Close();
			//logWriter = null;

			//smallLogRestartTimer.Stop();
			//smallLogWriter.Close();
			//smallLogWriter = null;

			//// unbind events? test if problem maybe....

			//_started = false;
		}


		// output is performed by adding lines that we want output to this queue here
		// and writing line by line from the queue when we have enough time.
		Queue<string> outputQ = new Queue<string>();
		bool writing = false;

		void StartSafeOutput()
		{
			Task.Factory.StartNew(() => {
				while (true) {
					if (outputQ.Count > 0) {
						string s = outputQ.Dequeue();
						writing = true;
						logWriter.WriteLine(s);
						writing = false;
					}
				}
			});
		}

		// output funcs

		public void Output()
		{
			Output("");
		}

		public void Output(string message)
		{
			try {
				if (LogOutput != null) {
					LogOutput(message);
				}

				message = DateTime.Now + ": " + message;

				// spam from server. heh
				if (message.Contains("<!-- Hosting24")) {
					message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
				}

				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(message);
				}

				//if (logWriter != null) {
				//	logWriter.WriteLine(message);
				//}
				outputQ.Enqueue(message);
			}
			catch (Exception e) {
				if (LogOutput != null) {
					LogOutput(e.ToString());
				}
				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(e.ToString());
				}
			}
		}

		public void SmallOutput(string message)
		{
			try {

				if (SmallLogOutput != null) {
					SmallLogOutput(message);
				}

				message = DateTime.Now + ": " + message;

				// spam from server. heh
				if (message.Contains("<!-- Hosting24")) {
					message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
				}

				if (smallLogWriter != null) {
					smallLogWriter.WriteLine(message);
				}
			}
			catch (Exception e) {
				if (LogOutput != null) {
					LogOutput(e.ToString());
				}
				if (ConsoleHelper.ConsolePresent) {
					Console.WriteLine(e.ToString());
				}
			}
		}
	}
}
