using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ViewModelBase> panels = new ObservableCollection<ViewModelBase>();
        public ObservableCollection<ViewModelBase> Panels
        {
            get
            {
                return panels;
            }

            set
            {
                panels = value;
            }
        }

        public MainWindowViewModel()
        {
            Systems.SystemManager.Init();
            panels.Add(new AllResourcesViewModel());
        }

    }
}
