using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class LogViewModel : ViewModelBase
    {
        private Log log;

        public Log Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        public LogViewModel()
        {
            // create new model
            Log = new Log();
            
            // init this
            LogMessageList = new ObservableCollection<LogMessage>();

            // event
            Log.LogOutput += Log_LogOutput;
        }

        #region === exposed from model ===

        /// <summary>
        /// expose model property in ViewModel
        /// </summary>
        public string Path
        {
            get
            {
                return Log.Path;
            }
            set
            {
                Log.Path = value;
            }
        }
        
        #endregion

        private int maxLogMessages = 3000;

        public int MaxLogMessages
        {
            get
            {
                return maxLogMessages;
            }

            set
            {
                maxLogMessages = value;
            }
        }

        public void TestMessage()
        {
            Log.Output("yoyoyo testestest1");
        }

        private void Log_LogOutput(string message)
        {
            KeepLogListShort();
            LogMessageList.Add(new LogMessage(message));
        }

        /// <summary>
        /// removes first messages from log list, keeping length less than MaxLogMessages
        /// </summary>
        void KeepLogListShort()
        {
            if (LogMessageList.Count > MaxLogMessages) {
                var dif = LogMessageList.Count - MaxLogMessages;
                for (int i = 0; i < dif; i++) {
                    LogMessageList.RemoveAt(0);
                }
            }
        }

        public ObservableCollection<LogMessage> LogMessageList { get; set; }

        /// <summary>
        /// clears log list in the interface
        /// </summary>
        public void Clear()
        {
            LogMessageList.Clear();
        }

    }
}