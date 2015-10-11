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
    public class LogViewModel : INotifyPropertyChanged
    {
        private Log log;

        #region === exposed from model ===

        /// <summary>
        /// expose model property in ViewModel
        /// </summary>
        public string Path
        {
            get
            {
                return log.Path;
            }
            set
            {
                log.Path = value;
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
            log.Output("yoyoyo testestest1");
        }

        /// <summary>
        /// constructor
        /// </summary>
        public LogViewModel()
        {
            // init this
            LogMessageList = new ObservableCollection<LogMessage>();

            // create new model
            log = new Log();

            // event
            log.LogOutput += Log_LogOutput;
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

        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// clears log list in the interface
        /// </summary>
        public void Clear()
        {
            LogMessageList.Clear();
        }

    }
}