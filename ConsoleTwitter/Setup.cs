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
		public static bool connectOnline = true;
		public static bool saveToDatabaseOrPHP = false;

		// show output in console.
		public static bool showOutput = true;
		
		// print output to log file.
		public static bool logOutput = true;
		public static string logPath = "log.txt";

		// connection data saved as strings
		public static string localConnectionString = @"server=localhost;userid=root;password=1234;database=hivemindcloud";
		public static string onlineConnectionString = @"server=mysql10.000webhost.com;userid=a3879893_admin;password=dumnezeu55;database=a3879893_tweet";
		public static string localPhpWordsLink = @"http://localhost/hhh/testing/postData.php";
		public static string onlinePhpWordsLink = @"http://hivemindcloud.hostoi.com/postData.php";
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
				
				Console.WriteLine("-c: Connection will be made to "
					+ (connectOnline ? "online. BEWARE: cannot send data directly with free plan!!!" : "localhost") 
					+ " . Toggle using -c");
				Console.WriteLine("-s: Data will be saved "
					+ (saveToDatabaseOrPHP ? "directly to database" : "through PHP post request")
					+ ". Toggle using -s");
				Console.WriteLine("-o: Console output is "
					+ showOutput
					+ ". Toggle using -o");
				Console.WriteLine("-l [<path>]: Logging to log file \"" + logPath + "\" is "
					+ (logOutput ? "on" : "off")
					+ ". Toggle using -l. Optional, use -l <path> to set new filename.");
				Console.WriteLine("-t <seconds>: Stream will run "
					+ (maxSecondsToRunStream == 0 ? "indefinitely"
						: "for maximum " + maxSecondsToRunStream + " seconds")
					+ ". Edit by using -t <seconds>, where <seconds> is a float. Use -t 0 for running indefinitely.");
				Console.WriteLine("-f <filter>: Set filter keywords for stream. Current filter: "
					+ filter + ". See Twitter API for details on how to use filter.");
				Console.WriteLine("run: Use command \"run\" to end setup and start program");
				command = Console.ReadLine();

				if (command == "-c") {
					connectOnline = !connectOnline;
					
				} else if (command == "-s") {
					saveToDatabaseOrPHP = !saveToDatabaseOrPHP;
					
				} else if (command == "-o") {
					showOutput = !showOutput;
					
				} else if (command == "-l") {
					logOutput = !logOutput;
					if (command.Length > 2) {
						string tempPath = command.Substring(3);
						try {
							string fullPath = Path.GetFullPath(tempPath);
							// if no error, path is valid
							if (tempPath.EndsWith(".txt")) {
								logPath = tempPath;
							} else {
								logPath = tempPath + ".txt";
							}
						}
						catch {
							Console.WriteLine("Path invalid. Try not to use fancy characters. Just letters and numbers");
						}
					}
					
				} else if (command.Substring(0, 2) == "-t") {
					float.TryParse(command.Substring(3), out maxSecondsToRunStream);
					if (maxSecondsToRunStream < 0) maxSecondsToRunStream = 0;
					
				} else if (command.Substring(0, 2) == "-f") {
					filter = command.Substring(3);
					
				}
			}

			// after while loop finished, all settings are ready and other modules should start.


			// start stream
			Stream.Start();

			// if logging, start log
			if (logOutput) {
				Log.Start();
			}

			// start database
			Database.Start();


		}


	}
}
