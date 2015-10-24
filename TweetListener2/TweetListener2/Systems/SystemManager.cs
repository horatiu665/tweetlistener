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
        private ObservableCollection<OldMainWindowViewModel> oldMainWindows = new ObservableCollection<OldMainWindowViewModel>();

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

        public ObservableCollection<OldMainWindowViewModel> OldMainWindowViewModels
        {
            get
            {
                return oldMainWindows;
            }
            set
            {
                oldMainWindows = value;
            }
        }

        private int StreamCount, RestCount, CredentialsCount, DatabaseCount, LogCount, KeywordDatabaseCount, 
            TweetDatabaseCount, QueryExpansionCount, PorterStemmerCount, MailHelperCount, OldMainWindowCount;

        public void Add<T>(T system)
        {
            if (system == null) return;

            Console.WriteLine("[SystemManager] Adding system " + system);

            if (system is StreamViewModel) {
                var sysRef = system as StreamViewModel;
                StreamViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = StreamCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is RestViewModel) {
                var sysRef = system as RestViewModel;
                RestViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = RestCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is CredentialsViewModel) {
                var sysRef = system as CredentialsViewModel;
                CredentialsViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = CredentialsCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is DatabaseViewModel) {
                var sysRef = system as DatabaseViewModel;
                DatabaseViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = DatabaseCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is LogViewModel) {
                var sysRef = system as LogViewModel;
                LogViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = LogCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is KeywordDatabaseViewModel) {
                var sysRef = system as KeywordDatabaseViewModel;
                KeywordDatabaseViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = KeywordDatabaseCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is TweetDatabaseViewModel) {
                var sysRef = system as TweetDatabaseViewModel;
                TweetDatabaseViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = TweetDatabaseCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is QueryExpansionViewModel) {
                var sysRef = system as QueryExpansionViewModel;
                QueryExpansionViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = QueryExpansionCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is PorterStemmerViewModel) {
                var sysRef = system as PorterStemmerViewModel;
                PorterStemmerViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = PorterStemmerCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is MailHelperViewModel) {
                var sysRef = system as MailHelperViewModel;
                MailHelperViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = MailHelperCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }

            if (system is OldMainWindowViewModel) {
                var sysRef = system as OldMainWindowViewModel;
                OldMainWindowViewModels.Add(sysRef);
                (system as ViewModelBase).CountInSystemManager = OldMainWindowCount++;
                AddedSystem(this, new SystemEventArgs(sysRef));
            }
        }

        private void AddedSystem(object sender, SystemEventArgs args)
        {
            if (OnAddedSystem != null) {
                OnAddedSystem(sender, args);
            }
        }

        public event EventHandler<SystemEventArgs> OnAddedSystem;

        public void Remove<T>(T system)
        {
            if (system == null) return;

            Console.WriteLine("[SystemManager] Removing system " + system);

            if (system is StreamViewModel) {
                var sysRef = system as StreamViewModel;
                StreamViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = StreamCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is RestViewModel) {
                var sysRef = system as RestViewModel;
                RestViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = RestCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is CredentialsViewModel) {
                var sysRef = system as CredentialsViewModel;
                CredentialsViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = CredentialsCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is DatabaseViewModel) {
                var sysRef = system as DatabaseViewModel;
                DatabaseViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = DatabaseCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is LogViewModel) {
                var sysRef = system as LogViewModel;
                LogViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = LogCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is KeywordDatabaseViewModel) {
                var sysRef = system as KeywordDatabaseViewModel;
                KeywordDatabaseViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = KeywordDatabaseCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is TweetDatabaseViewModel) {
                var sysRef = system as TweetDatabaseViewModel;
                TweetDatabaseViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = TweetDatabaseCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is QueryExpansionViewModel) {
                var sysRef = system as QueryExpansionViewModel;
                QueryExpansionViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = QueryExpansionCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is PorterStemmerViewModel) {
                var sysRef = system as PorterStemmerViewModel;
                PorterStemmerViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = PorterStemmerCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
            if (system is MailHelperViewModel) {
                var sysRef = system as MailHelperViewModel;
                MailHelperViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = MailHelperCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }

            if (system is OldMainWindowViewModel) {
                var sysRef = system as OldMainWindowViewModel;
                OldMainWindowViewModels.Remove(sysRef);
                (system as ViewModelBase).CountInSystemManager = OldMainWindowCount--;
                RemovedSystem(this, new SystemEventArgs(sysRef));
            }
        }

        private void RemovedSystem(object sender, SystemEventArgs args)
        {
            if (OnRemovedSystem != null) {
                OnRemovedSystem(sender, args);
            }
        }

        public event EventHandler<SystemEventArgs> OnRemovedSystem;

    }
}
