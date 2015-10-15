using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    /// <summary>
    /// class used in the menu of AllResources. Specifies type of resource
    /// </summary>
    public class ResourceListItem
    {
        private ViewModelBase resource;
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
                return resource.ToString();
                // should make this prettier - for each system, give it a nice name
            }
        }

        public ResourceListItem(ViewModelBase type)
        {
            resource = type;
        }

    }
}
