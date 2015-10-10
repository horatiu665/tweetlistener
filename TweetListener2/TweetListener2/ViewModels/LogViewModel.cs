using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    public class LogViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LogMessage> LogMessageList = new ObservableCollection<LogMessage>();

        public event PropertyChangedEventHandler PropertyChanged;

        public object ClearLogButtonClick;

        internal void Clear()
        {
            LogMessageList.Clear();
        }
    }
}