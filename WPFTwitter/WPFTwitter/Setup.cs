using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WPFTwitter
{
	/// <summary>
	/// This class allows the user to setup various settings, and to start the streaming program
	/// </summary>
	public static class Setup
	{
		public static bool connectOnline = false;
		public static bool saveToDatabaseOrPhp = false;
		public static bool startDatabase = true;

		// print output to log file.
		public static bool logOutput = true;
		public static string logPath = "log.txt";

		// max time to run Stream program before it closes.
		public static float maxSecondsToRunStream = 0;

		// max tweets to receive before stopping stream.
		public static int maxTweetsToRunStream = 0;

		
		/// <summary>
		/// the main part of the setup module. user is prompted with stuff.
		/// </summary>
		public static void StartConsole()
		{
			ConsoleHelper.Create();

			string command = "";
			while (command != "run") {
				if (command != "help") {
					// connection online/local
					Console.WriteLine("c: \t" + "Connection made to "
						+ (connectOnline ? "online" : "localhost")
						);
					// save thru PHP/direct
					Console.WriteLine("s: \t" + "Data saved "
						+ (saveToDatabaseOrPhp ? "directly to database" : "through PHP post request")
						);
					// log on/off and path
					Console.WriteLine("l <path>: \t" + "Logging to log file \"" + logPath + "\" is "
						+ (logOutput ? "on" : "off")
						);
					// time to run stream, 0=infinite
					Console.WriteLine("t [s <seconds>] [t <tweets>]: \t" + "Stream will run "
						+ (maxTweetsToRunStream == 0 ? (
							(maxSecondsToRunStream == 0 ? "indefinitely"
								: "for max " + maxSecondsToRunStream + " seconds")
							) : "until " + maxTweetsToRunStream + " tweets are received")
							);
					// filter keywords
					Console.WriteLine("f <filter>: \t" + "Current filter: " + Stream.Filter
						);

					Console.WriteLine("d: \t" + "Save to database toggle: " + startDatabase
						);
					Console.WriteLine("help: \t Use help for instructions");
					// "run" to start
					Console.WriteLine("run: \t Use run to start");
					Console.WriteLine("q: \t Quit");
				}

				Console.Write("> ");
				command = Console.ReadLine();
				Console.WriteLine();

				if (command.Length > 0) {
					if (command == "c") {
						connectOnline = !connectOnline;

					} else if (command == "s") {
						saveToDatabaseOrPhp = !saveToDatabaseOrPhp;

					} else if (command.Substring(0, 1) == "l") {
						logOutput = !logOutput;
						if (command.Length > 1) {
							string tempPath = command.Substring(2);
							logPath = Log.ValidatePath(tempPath);
						}

					} else if (command.Substring(0, 1) == "t") {
						if (command.Substring(2, 1) == "s") {
							float.TryParse(command.Substring(4), out maxSecondsToRunStream);
							if (maxSecondsToRunStream < 0) {
								maxSecondsToRunStream = 0;
							}
							if (maxSecondsToRunStream > 0) {
								maxTweetsToRunStream = 0;
							}
						} else if (command.Substring(2, 1) == "t") {
							int.TryParse(command.Substring(4), out maxTweetsToRunStream);
							if (maxTweetsToRunStream < 0) {
								maxTweetsToRunStream = 0;
							}
							if (maxTweetsToRunStream > 0) {
								maxSecondsToRunStream = 0;
							}
						} else {
							Console.WriteLine("Argument unrecognized. Use > t s <seconds> for max seconds, or >t <tweets> for max tweets.");
						}

					} else if (command.Substring(0, 1) == "f") {
						Stream.Filter = command.Substring(2);

					} else if (command.Substring(0, 1) == "d") {
						startDatabase = !startDatabase;

					} else if (command == "run") {
						Console.WriteLine("Running...");

					} else if (command == "help") {
						// write instructions for all commands
						// connection online/local
						Console.WriteLine("c: \n\t" + "Connection made to "
						+ (connectOnline ? "online" : "localhost")
						+ "\n\t" + "Change connection type: localhost/online");
						// save thru PHP/direct
						Console.WriteLine("s: \n\t" + "Data saved "
						+ (saveToDatabaseOrPhp ? "directly to database" : "through PHP post request")
						+ "\n\t" + "Change type of data saving: directly to database/through PHP POST request");
						// log on/off and path
						Console.WriteLine("l <path>: \n\t" + "Logging to log file \"" + logPath + "\" is "
							+ (logOutput ? "on" : "off")
							+ "\n\t" + "Use > l <path> to set new filename.");
						// time to run stream, 0=infinite
						Console.WriteLine("t [s <seconds>] [t <tweets>]: \n\t" + "Stream will run "
							+ (maxTweetsToRunStream == 0 ? (
								(maxSecondsToRunStream == 0 ? "indefinitely"
									: "for max " + maxSecondsToRunStream + " seconds")
								) : "until " + maxTweetsToRunStream + " tweets are received")
							+ ". \n\t" + "Use > t s <seconds>, where <seconds> is a float."
							+ ". \n\t" + "Use > t s 0 for running indefinitely."
							+ ". \n\t" + "Use > t t <tweets> for running until <tweets> tweets are received."
							);
						// filter keywords
						Console.WriteLine("f <filter>: \n\t" + "Set filter keywords for stream. Current filter:\n\t"
							+ Stream.Filter + "\n\t" + "See Twitter API for details on how to use filter.");
						// "help" to see info
						Console.WriteLine("help: \n\t" + "See this info");
						// "run" to start
						Console.WriteLine("run: \n\t" + "Use command \"run\" to end setup and start program");

					} else if (command == "q") {
						ConsoleHelper.Destroy();
						return;
					} else {
						Console.WriteLine("Command not recognized");
					}
				}

				Console.WriteLine();

			}

			// after while loop finished, all settings are ready and other modules should start.


			// start stream
			Stream.Start(Stream.Filter);

			// if logging, start log
			if (logOutput) {
				Log.Start(logPath);
			}

			if (startDatabase) {
				// start database
				DatabaseSaver.Start(connectOnline, saveToDatabaseOrPhp);
			}

			//Task.Factory.StartNew(() => {
			//	while (true) {
			//		if (Console.KeyAvailable) {
			//			var r = Console.ReadKey();
			//			if (r.Key == ConsoleKey.Escape) {
			//				ConsoleHelper.Destroy();
			//				return;
			//			}
			//		}
			//	}
			//});
		}


	}
}
