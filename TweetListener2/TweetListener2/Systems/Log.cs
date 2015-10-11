using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TweetListener2.Systems
{
    /// <summary>
    /// used to log messages to a log file and potentially to the console (not thoroughly tested)
    /// </summary>
    public class Log
    {
        /// <summary>
        /// writes to file
        /// </summary>
        StreamWriter logWriter;

        /// <summary>
        /// path for StreamWriter
        /// </summary>
        string path;

        /// <summary>
        /// path for StreamWriter - validated
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                var validPath = ValidatePath(value);
                if (path != validPath) {
                    path = validPath;
                }
            }
        }

        /// <summary>
        /// is log running? is the reset timer active?
        /// </summary>
        bool started = false;

        /// <summary>
        /// how often to restart log to make sure log file is not lost due to some strange error or crash? in millis
        /// </summary>
        double restartLogIntervalMillis = 30000;

        /// <summary>
        /// timer makes sure that the log restarts once in a while to prevent crash data loss
        /// </summary>
        Timer logRestartTimer;

        /// <summary>
        /// event every time log writes to file
        /// </summary>
        public event Action<string> LogOutput;

        /// <summary>
        /// constructor
        /// </summary>
        public Log(string path = "log.txt")
        {
            this.Path = path;
        }

        /// <summary>
        /// happens when log restart timer elapsed after restartLogIntervalMillis
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void RestartTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (logWriter != null) {
                logWriter.Close();
                logWriter = new StreamWriter(Path, true);
            } else {
                if (logRestartTimer != null) {
                    logRestartTimer.Stop();
                }
                Start();
            }
        }

        /// <summary>
        /// is path to log file valid? returns valid path (ending in "txt") or default ("log.txt")
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        string ValidatePath(string path)
        {
            try {
                string fullPath = System.IO.Path.GetFullPath(path);
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
        /// starts log = validates path and initializes restart timer
        /// </summary>
        /// <param name="path"></param>
        void Start()
        {
            if (!started) {
                started = true;

                // open log writer
                logWriter = new StreamWriter(Path, true);
                Output("New log started");

                // once in a while stop and start log, so we do not lose all the data in the log.
                logRestartTimer = new System.Timers.Timer(restartLogIntervalMillis);
                logRestartTimer.Start();
                logRestartTimer.Elapsed += RestartTimerElapsed;

            }
        }

        /// <summary>
        /// stops log and restart timer
        /// </summary>
        void Stop()
        {
            if (started) {
                started = false;
                Output("Log stopped at " + Path);
                logRestartTimer.Elapsed -= RestartTimerElapsed;
                logRestartTimer.Stop();
                logWriter.Close();
            }
        }

        /// <summary>
        /// outputs log message to file and console, and calls event for other elements to receive log message.
        /// </summary>
        /// <param name="message"></param>
        public void Output(string message)
        {
            Start();

            try {
                if (LogOutput != null) {
                    LogOutput(message);
                }

                message = DateTime.Now + ": " + message;

                //// spam from server. heh
                //if (message.Contains("<!-- Hosting24")) {
                //    message = message.Substring(0, message.IndexOf("<!-- Hosting24"));
                //}

                if (ConsoleHelper.ConsolePresent) {
                    Console.WriteLine(message);
                }

                if (logWriter != null) {
                    logWriter.WriteLine(message);
                }
                //outputQ.Enqueue(message);

                //SafeOutput();
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
