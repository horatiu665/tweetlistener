using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    public class MainWindowViewModel
    {
        public LogViewModel Log { get; set; }

        public MainWindowViewModel()
        {
            Log = new LogViewModel();
        }
    }
}
