using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.ViewModels;

namespace TweetListener2.Systems
{
    public class AddedSystemEventArgs : EventArgs
    {
        public ViewModelBase system;
        public AddedSystemEventArgs(ViewModelBase system)
        {
            this.system = system;
        }
    }
}
