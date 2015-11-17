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
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public abstract string Name { get; }

        public int CountInSystemManager { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null) {
                PropertyChanged(sender, args);
            }
        }
    }
}
