using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class AllResourcesViewModel : ViewModelBase
    {


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

        /// <summary>
        /// constructor
        /// </summary>
        public AllResourcesViewModel()
        {

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
    }
}
