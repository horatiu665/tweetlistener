using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleTwitter
{
	/// <summary>
	/// handles listening to events and printing out logs to the console and to log files.
	/// </summary>
	public static class Log
	{
		static StreamWriter logWriter;
		public static string logPath = "log.txt";

		/// <summary>
		/// Log module starts here
		/// </summary>
		public static void Start()
		{
			// open log writer
			logWriter = new StreamWriter(logPath, true);
			
		}

		public static void Output()
		{
			Output("");
		}

		public static void Output(string message)
		{
			// showOutput message should be written even if showOutput is false.
			if (message.Contains("showOutput")) {
				Console.WriteLine(message);
				if (logWriter != null) {
					logWriter.WriteLine(message);
				}
			}

			if (message.Contains("<!-- Hosting24")) {
				message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
			}

			Console.WriteLine(message);
			
			if (logWriter != null) {
				logWriter.WriteLine(message);
			}
		}
	}
}
