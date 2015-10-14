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
    public class ViewModelBase : INotifyPropertyChanged
    {
        public string Name
        {
            get
            {
                return GetType().ToString();
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
