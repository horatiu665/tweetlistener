using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ConsoleTwitter
{
	/// <summary>
	/// This class allows the user to setup various settings, and to start the streaming program
	/// </summary>
	public static class Setup
	{
		public static bool connectOnline = false;
		public static bool saveToDatabaseOrPHP = false;

		// print output to log file.
		public static bool logOutput = true;
		public static string logPath = "log.txt";

		// connection data saved as strings
		public static string localConnectionString = @"server=localhost;userid=root;password=1234;database=hivemindcloud";
		public static string onlineConnectionString = @"server=mysql10.000webhost.com;userid=a3879893_admin;password=dumnezeu55;database=a3879893_tweet";
		public static string localPhpTweetsLink = @"http://localhost/hhh/testing/saveTweet.php";
		public static string onlinePhpTweetsLink = @"http://hivemindcloud.hostoi.com/saveTweet.php";

		// max time to run Stream program before it closes.
		public static float maxSecondsToRunStream = 0;

		/// <summary>
		/// filter used for twitter stream
		/// </summary>
		public static string filter = "";

		/// <summary>
		/// the main part of the setup module. user is prompted with stuff.
		/// </summary>
		public static void StartConsole()
		{
			string command = "";
			while (command != "run") {
				if (command != "help") {
					// connection online/local
					Console.WriteLine("c: \t" + "Connection made to "
						+ (connectOnline ? "online" : "localhost")
						);
					// save thru PHP/direct
					Console.WriteLine("s: \t" + "Data saved "
						+ (saveToDatabaseOrPHP ? "directly to database" : "through PHP post request")
						);
					// log on/off and path
					Console.WriteLine("l [<path>]: \t" + "Logging to log file \"" + logPath + "\" is "
						+ (logOutput ? "on" : "off")
						);
					// time to run stream, 0=infinite
					Console.WriteLine("t <seconds>: \t" + "Stream will run "
						+ (maxSecondsToRunStream == 0 ? "indefinitely"
							: "for max " + maxSecondsToRunStream + " seconds")
							);
					// filter keywords
					Console.WriteLine("f <filter>: \t" + "Current filter: " + filter
						);
					Console.WriteLine("help: \t Use help for instructions");
					// "run" to start
					Console.WriteLine("run: \t Use run to start");
				}

				Console.Write("> ");
				command = Console.ReadLine();
				Console.WriteLine();

				if (command.Length > 0) {
					if (command == "c") {
						connectOnline = !connectOnline;

					} else if (command == "s") {
						saveToDatabaseOrPHP = !saveToDatabaseOrPHP;

					} else if (command.Substring(0, 1) == "l") {
						logOutput = !logOutput;
						if (command.Length > 1) {
							string tempPath = command.Substring(2);
							logPath = Log.ValidatePath(tempPath);
						}

					} else if (command.Substring(0, 1) == "t") {
						float.TryParse(command.Substring(2), out maxSecondsToRunStream);
						if (maxSecondsToRunStream < 0) maxSecondsToRunStream = 0;

					} else if (command.Substring(0, 1) == "f") {
						filter = command.Substring(2);

					} else if (command == "run") {
						Console.WriteLine("Running...");

					} else if (command == "help") {
						// write instructions for all commands
						// connection online/local
						Console.WriteLine("-c: \n\t" + "Connection made to "
						+ (connectOnline ? "online" : "localhost")
						+ "\n\t" + "Change connection type: localhost/online");
						// save thru PHP/direct
						Console.WriteLine("-s: \n\t" + "Data saved "
						+ (saveToDatabaseOrPHP ? "directly to database" : "through PHP post request")
						+ "\n\t" + "Change type of data saving: directly to database/through PHP POST request");
						// log on/off and path
						Console.WriteLine("-l [<path>]: \n\t" + "Logging to log file \"" + logPath + "\" is "
							+ (logOutput ? "on" : "off")
							+ "\n\t" + "Use -l <path> to set new filename.");
						// time to run stream, 0=infinite
						Console.WriteLine("-t <seconds>: \n\t" + "Stream will run "
							+ (maxSecondsToRunStream == 0 ? "indefinitely"
								: "for max " + maxSecondsToRunStream + " seconds")
							+ ". \n\t" + "Use -t <seconds>, where <seconds> is a float. \n\t" + "Use -t 0 for running indefinitely.");
						// filter keywords
						Console.WriteLine("-f <filter>: \n\t" + "Set filter keywords for stream. Current filter:\n\t"
							+ filter + "\n\t" + "See Twitter API for details on how to use filter.");
						// "help" to see info
						Console.WriteLine("help: \n\t" + "See this info");
						// "run" to start
						Console.WriteLine("run: \n\t" + "Use command \"run\" to end setup and start program");

					} else {
						Console.WriteLine("Command not recognized");
					}
				}

				Console.WriteLine();

			}

			// after while loop finished, all settings are ready and other modules should start.


			// start stream
			Stream.Start(filter);

			// if logging, start log
			if (logOutput) {
				Log.Start(logPath);
			}

			// start database
			DatabaseSaver.Start();

			// start viewer
			Viewer.Start(maxSecondsToRunStream);

		}


	}
}
