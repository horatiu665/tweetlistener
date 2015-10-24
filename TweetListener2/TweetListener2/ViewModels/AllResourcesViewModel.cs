using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class AllResourcesViewModel : ViewModelBase
    {
        #region ViewModel lists

        /*
        public ObservableCollection<StreamViewModel> StreamViewModels
        {
            get
            {
                return SystemManager.instance.StreamViewModels;
            }
            
        }

        public ObservableCollection<RestViewModel> RestViewModels
        {
            get
            {
                return SystemManager.instance.RestViewModels;
            }
            
        }

        public ObservableCollection<CredentialsViewModel> CredentialsViewModels
        {
            get
            {
                return SystemManager.instance.CredentialsViewModels;
            }
            
        }

        public ObservableCollection<DatabaseViewModel> DatabaseViewModels
        {
            get
            {
                return SystemManager.instance.DatabaseViewModels;
            }
            
        }

        public ObservableCollection<LogViewModel> LogViewModels
        {
            get
            {
                return SystemManager.instance.LogViewModels;
            }
            
        }

        public ObservableCollection<KeywordDatabaseViewModel> KeywordDatabaseViewModels
        {
            get
            {
                return SystemManager.instance.KeywordDatabaseViewModels;
            }
            
        }

        public ObservableCollection<TweetDatabaseViewModel> TweetDatabaseViewModels
        {
            get
            {
                return SystemManager.instance.TweetDatabaseViewModels;
            }
            
        }

        public ObservableCollection<QueryExpansionViewModel> QueryExpansionViewModels
        {
            get
            {
                return SystemManager.instance.QueryExpansionViewModels;
            }
            
        }

        public ObservableCollection<PorterStemmerViewModel> PorterStemmerViewModels
        {
            get
            {
                return SystemManager.instance.PorterStemmerViewModels;
            }
            
        }

        public ObservableCollection<MailHelperViewModel> MailHelperViewModels
        {
            get
            {
                return SystemManager.instance.MailHelperViewModels;
            }
            
        }

        //*/
        #endregion

        private ObservableCollection<ResourceListItem> resourceList = new ObservableCollection<ResourceListItem>();

        public ObservableCollection<ResourceListItem> ResourceList
        {
            get
            {
                return resourceList;
            }

            set
            {
                resourceList = value;
            }
        }

        public override string Name
        {
            get
            {
                return "All Resources";

            }
        }

        internal void SortResourceList_Click(object sender, RoutedEventArgs e)
        {
            var sortedList = new List<ResourceListItem>(resourceList.OrderBy(rli => rli.Name));
            ResourceList.Clear();
            for (int i = 0; i < sortedList.Count; i++) {
                ResourceList.Add(sortedList[i]);
            }
        }

        internal void CreateOldTweetListenerBatch_Click(object sender, RoutedEventArgs e, string batchText)
        {
            List<List<string>> batches = new List<List<string>>();
            foreach (var s in batchText.Split('\n', '\r')) {
                if (s != "") {
                    var str = s.Replace("^", "");
                    str = str.Trim();
                    if (str.Substring(0, Math.Min(5, str.Length)) == "start") {
                        batches.Add(new List<string>());
                    }

                    batches.Last().Add(str);
                }
            }

            // now we basically have a batch file reader based on the old batch file format = still same retardo method but at least within same application = improvement

            foreach (List<string> batchList in batches) {
                SystemManager.instance.Add(new OldMainWindowViewModel(batchList));
            }
        }

        internal void AddOldTweetListener_Click(object sender, RoutedEventArgs e)
        {
            SystemManager.instance.Add(new OldMainWindowViewModel());
        }

        /// <summary>
        /// constructor
        /// </summary>
        public AllResourcesViewModel()
        {
            SystemManager.instance.OnAddedSystem += SystemManager_OnAddedSystem;
        }

        private void SystemManager_OnAddedSystem(object sender, SystemEventArgs e)
        {
            resourceList.Add(new ResourceListItem(e.system));

        }

        public void AddNewStream_Click()
        {

        }
        public void AddNewRest_Click()
        {

        }
        public void AddNewCredentials_Click()
        {

        }
        public void AddNewDatabase_Click()
        {

        }
        public void AddNewLog_Click()
        {

        }
        public void AddNewKeywordDatabase_Click()
        {

        }
        public void AddNewTweetDatabase_Click()
        {

        }
        public void AddNewQueryExpansion_Click()
        {

        }
        public void AddNewPorterStemmer_Click()
        {

        }
        public void AddNewMailHelper_Click()
        {

        }

        /// <summary>
        /// create systems and connect them
        /// </summary>
        public void AddNewTweetListener_Click()
        {

            Console.WriteLine("Not supported yet");
            return;

            SystemManager sysMan = SystemManager.instance;

            var log = new LogViewModel();
            sysMan.Add(log);

            var credentials = new CredentialsViewModel(log);
            sysMan.Add(credentials);

            var keywordDatabase = new KeywordDatabaseViewModel(log);
            sysMan.Add(keywordDatabase);

            var database = new DatabaseViewModel(log);
            sysMan.Add(database);

            var tweetDatabase = new TweetDatabaseViewModel(database);
            sysMan.Add(tweetDatabase);

            var rest = new RestViewModel(database, log, tweetDatabase);
            sysMan.Add(rest);

            var stream = new StreamViewModel(database, log, rest, keywordDatabase, tweetDatabase);
            sysMan.Add(stream);

        }

        public void ResourceListItem_Click(object sender, RoutedEventArgs e)
        {
            var resourceListItem = (ResourceListItem)((Button)sender).DataContext;
            if (resourceListItem != null) {
                // create viewmodel and whatnot in mainwindow
                MainWindowViewModel.instance.AddPanel(resourceListItem.Resource);
            }
        }
    }
}
