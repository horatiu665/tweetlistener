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
        public static MainWindowViewModel instance;

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

        public override string Name
        {
            get
            {
               return "Main Window";
            }
        }

        public MainWindowViewModel()
        {
            instance = this;

            Systems.SystemManager.Init();
            panels.Add(new AllResourcesViewModel());

        }

        public void AddPanel(ViewModelBase panel)
        {
            panels.Add(panel);
        }

        public void RemovePanel(ViewModelBase panel)
        {
            panels.Remove(panel);
        }

    }
}
