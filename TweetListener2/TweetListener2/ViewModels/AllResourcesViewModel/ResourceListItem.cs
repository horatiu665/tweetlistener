using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    /// <summary>
    /// class used in the menu of AllResources. Specifies type of resource
    /// </summary>
    public class ResourceListItem : INotifyPropertyChanged
    {
        private ViewModelBase resource;

        public event PropertyChangedEventHandler PropertyChanged;

        public ViewModelBase Resource
        {
            get
            {
                return resource;
            }
        }

        public string Name
        {
            get
            {
                return resource.Name;
            }
        }

        public int CountInSystemManager
        {
            get
            {
                return resource.CountInSystemManager;
            }
        }

        public ResourceListItem(ViewModelBase type)
        {
            resource = type;
            type.PropertyChanged += Resource_PropertyChanged;
        }

        private void Resource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //if (PropertyChanged != null) {
                PropertyChanged(sender, e);
            //}
        }
    }
}
