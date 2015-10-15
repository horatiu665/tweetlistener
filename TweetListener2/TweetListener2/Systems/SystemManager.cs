using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.ViewModels;

namespace TweetListener2.Systems
{
    /// <summary>
    /// Model. Keeps track of all the systems: Streams, Rest, Databases, Logs, etc.
    /// </summary>
    public class SystemManager
    {
        private SystemManager()
        {
            if (instance == null) {
                instance = this;
            }
        }

        public static void Init()
        {
            new SystemManager();
        }

        public static SystemManager instance;

        private ObservableCollection<StreamViewModel> streams = new ObservableCollection<StreamViewModel>();
        private ObservableCollection<RestViewModel> rests = new ObservableCollection<RestViewModel>();
        private ObservableCollection<CredentialsViewModel> credentials = new ObservableCollection<CredentialsViewModel>();
        private ObservableCollection<DatabaseViewModel> databases = new ObservableCollection<DatabaseViewModel>();
        private ObservableCollection<LogViewModel> logs = new ObservableCollection<LogViewModel>();
        private ObservableCollection<KeywordDatabaseViewModel> keywordDatabases = new ObservableCollection<KeywordDatabaseViewModel>();
        private ObservableCollection<TweetDatabaseViewModel> tweetDatabases = new ObservableCollection<TweetDatabaseViewModel>();
        private ObservableCollection<QueryExpansionViewModel> queryExpansions = new ObservableCollection<QueryExpansionViewModel>();
        private ObservableCollection<PorterStemmerViewModel> porterStemmers = new ObservableCollection<PorterStemmerViewModel>();
        private ObservableCollection<MailHelperViewModel> mailHelpers = new ObservableCollection<MailHelperViewModel>();

        public ObservableCollection<StreamViewModel> StreamViewModels
        {
            get
            {
                return streams;
            }

            set
            {
                streams = value;
            }
        }

        public ObservableCollection<RestViewModel> RestViewModels
        {
            get
            {
                return rests;
            }

            set
            {
                rests = value;
            }
        }

        public ObservableCollection<CredentialsViewModel> CredentialsViewModels
        {
            get
            {
                return credentials;
            }

            set
            {
                credentials = value;
            }
        }

        public ObservableCollection<DatabaseViewModel> DatabaseViewModels
        {
            get
            {
                return databases;
            }

            set
            {
                databases = value;
            }
        }

        public ObservableCollection<LogViewModel> LogViewModels
        {
            get
            {
                return logs;
            }

            set
            {
                logs = value;
            }
        }

        public ObservableCollection<KeywordDatabaseViewModel> KeywordDatabaseViewModels
        {
            get
            {
                return keywordDatabases;
            }

            set
            {
                keywordDatabases = value;
            }
        }

        public ObservableCollection<TweetDatabaseViewModel> TweetDatabaseViewModels
        {
            get
            {
                return tweetDatabases;
            }

            set
            {
                tweetDatabases = value;
            }
        }

        public ObservableCollection<QueryExpansionViewModel> QueryExpansionViewModels
        {
            get
            {
                return queryExpansions;
            }

            set
            {
                queryExpansions = value;
            }
        }

        public ObservableCollection<PorterStemmerViewModel> PorterStemmerViewModels
        {
            get
            {
                return porterStemmers;
            }

            set
            {
                porterStemmers = value;
            }
        }

        public ObservableCollection<MailHelperViewModel> MailHelperViewModels
        {
            get
            {
                return mailHelpers;
            }

            set
            {
                mailHelpers = value;
            }
        }

        public void Add<T>(T system)
        {
            if (system == null) return;

            Console.WriteLine("[SystemManager] Adding system " + system);

            if (system is StreamViewModel) {
                var sysRef = system as StreamViewModel;
                StreamViewModels.Add(sysRef);
                AddedSystem(this, new AddedSystemEventArgs(sysRef));
            }
            if (system is LogViewModel) {
                var sysRef = system as LogViewModel;
                LogViewModels.Add(sysRef);
                AddedSystem(this, new AddedSystemEventArgs(sysRef));
            }
        }

        private void AddedSystem(object sender, AddedSystemEventArgs args)
        {
            if (OnAddedSystem != null) {
                OnAddedSystem(sender, args);
            }
        }

        public event EventHandler<AddedSystemEventArgs> OnAddedSystem;
    }
}
