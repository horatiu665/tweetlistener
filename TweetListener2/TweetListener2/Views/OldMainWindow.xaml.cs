using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Net.Mail;
using System.Net;

using TweetListener2.Systems;
using TweetListener2.ViewModels;


namespace TweetListener2.Views
{
    /// <summary>
    /// Interaction logic for OldMainWindow.xaml
    /// </summary>
    public partial class OldMainWindow : UserControl, INotifyPropertyChanged
    {
        private OldMainWindowViewModel viewModel;

        public OldMainWindowViewModel vm
        {
            get
            {
                if (viewModel != null) {
                    return viewModel;
                } else {
                    if (DataContext is ViewModels.OldMainWindowViewModel) {
                        viewModel = (ViewModels.OldMainWindowViewModel)DataContext;
                        return viewModel;

                    } else {

                        return null;
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LogMessageListCollection LogMessageList
        {
            get { return vm.LogMessageList; }
        }
        public LogMessageListCollection RestMessageList
        {
            get { return vm.RestMessageList; }
        }

        public bool ContinuousTweetViewUpdate
        {
            get { return vm.ContinuousTweetViewUpdate; }
            set { vm.ContinuousTweetViewUpdate = value; }
        }

        public bool EmailSpammer
        {
            get { return vm.EmailSpammer; }
            set { vm.EmailSpammer = value; }
        }

        public TimeSpan EmailSpammerTimeSpan
        {
            get { return vm.EmailSpammerTimeSpan; }
            set { vm.EmailSpammerTimeSpan = value; }
        }

        public bool EmailSpammerPositive
        {
            get { return vm.EmailSpammerPositive; }
            set { vm.EmailSpammerPositive = value; }
        }

        public TimeSpan EmailSpammerPositiveTimeSpan
        {
            get { return vm.EmailSpammerPositiveTimeSpan; }
            set { vm.EmailSpammerPositiveTimeSpan = value; }
        }

        public string WindowTitle
        {
            get { return Name; }
            set
            {
                Name = value;
            }
        }

        public int DatabaseDestinationComboBoxIndex
        {
            get
            {
                return vm.DatabaseDestinationComboBoxIndex;
            }
            set
            {
                vm.DatabaseDestinationComboBoxIndex = value;
            }
        }

        public string TweetsPerSecond
        {
            get
            {
                return vm.TweetsPerSecond.ToString("F2") + " tps; " + (vm.TweetsPerSecond * 60).ToString("F0") + " tpm; " + (vm.TweetsPerSecond * 3600).ToString("F0") + " tph";
            }
        }

        public string FromFileLoader
        {
            get
            {
                return vm.FromFile_isLoading
                            ? vm.FromFile_Loaded
                                ? "Working... percent done: "
                                    + fromFile_percentDone.ToString("F3")
                                : "Reading lines... percent done: "
                                    + fromFile_percentDone.ToString("F3")
                            : "Idle"
                                ;

            }
        }

        /// <summary>
        /// 0 to 1, percentage of tweets from file added to tweet list.
        /// </summary>
        float fromFile_percentDone = 0f;


        /*
        ScrollToLast
        OnlyEnglish
        OnlyWithHashtags
        FromFileLoader
        TextFileDatabasePath
        DatabaseTableName
        ConnectionString
        SaveToDatabase
        MaxTweetDatabaseSendRetries
        SaveToTextFileProperty
        SaveToRamProperty
        CountersOn
        OutputDatabaseMessages
        LogEveryJson
        NaiveExpansionPercentage
        NaiveExpansionGenerations
        EfronMu
        RankNHashtags
        ExpandOnlyOnCooccurring
        ApplyFeedback
        LogModels
        ExpansionProgressLabel
        ContinuousUpdate
        KeywordList
        CurrentCredentialsIndex
        ScrollToLast
        */

        public OldMainWindow()
        {
            InitializeComponent();
            this.Loaded += OldMainWindow_Loaded;
        }

        private void OldMainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("Starting to initialize bindings YOYOYOYYOYO");
            InitializeBindings();
        }

        /// <summary>
        /// cannot be called from constructor because viewModel is not ready (but it is ready after View constructor finishes, tested using a little button printing DataContext in ViewSpawner)
        /// </summary>
        public void InitializeBindings()
        {
            //////// initialize bindings
            // log binding
            logPane.DataContext = vm.Log;
            logView.DataContext = vm.LogMessageList;
            vm.Log.LogOutput += vm.Log_LogOutput;
            checkBox_logCounters.DataContext = vm.Stream;
            checkBox_streamJsonSpammer.DataContext = vm.Stream;
            checkBox_databaseMessages.DataContext = vm.DatabaseSaver;
            databaseRetries.DataContext = vm.DatabaseSaver;
            checkBox_saveToTextFile.DataContext = vm.DatabaseSaver;
            checkBox_saveToRam.DataContext = vm.TweetDatabase;
            checkBox_database.DataContext = vm.DatabaseSaver;
            database_textFileDbPathTextBox.DataContext = vm.DatabaseSaver;
            setCredentialsDefault.DataContext = vm.Credentials;
            database_tableNameTextBox.DataContext = vm.DatabaseSaver;
            startStreamButton.DataContext = vm.Stream;
            database_connectionString.DataContext = vm.DatabaseSaver;

            // rest log binding
            restView.DataContext = vm.RestMessageList;

            // kData list binding
            //keywordListView.ItemsSource = keywordDatabase.KeywordList;
            keywordView.DataContext = vm.KeywordDatabase;

            expansionPanel.DataContext = vm.QueryExpansion;

            vm.AutoExpansionTimer.Elapsed += (s, a) => {
                vm.Log.Output("AutoExpansionTimer elapsed");
                AutoExpand();
            };
            // every 10 minutes expand.
            //autoExpansionTimer.Interval = 10*60*1000;



            // tweet view binding
            tweetView.DataContext = vm.TweetDatabase.Tweets;
            tweetViewContinuousUpdate.DataContext = vm;
            tweetViewOnlyEnglish.DataContext = vm.TweetDatabase;
            tweetViewOnlyWithHashtags.DataContext = vm.TweetDatabase;

            // saving tweets from Streaming and Rest into TweetDatabase
            vm.Stream.stream.MatchingTweetReceived += (s, a) => {
                Task.Factory.StartNew(() => {
                    vm.TweetDatabase.AddTweet(new TweetData(a.Tweet, TweetData.Sources.Stream, 0, 1));
                    vm.UpdateTweetsNextUpdate = true;

                });
                vm.TweetsLastSecond++;
            };
            vm.Stream.sampleStream.TweetReceived += (s, a) => {
                Task.Factory.StartNew(() => {
                    vm.TweetDatabase.AddTweet(new TweetData(a.Tweet, TweetData.Sources.Stream, 0, 1));
                    vm.UpdateTweetsNextUpdate = true;

                });
                vm.TweetsLastSecond++;
            };

            vm.Rest.TweetFound += (t) => {
                Task.Factory.StartNew(() => {
                    vm.TweetDatabase.AddTweet(new TweetData(t, TweetData.Sources.Rest, 0, 1));
                    vm.UpdateTweetsNextUpdate = true;

                });
                vm.TweetsLastSecond++;
            };

            vm.TweetViewUpdateTimer.Interval = 1000;
            vm.TweetViewUpdateTimer.Elapsed += (s, a) => {
                UpdateTweetView();
            };
            vm.TweetViewUpdateTimer.Start();


            vm.TweetsPerSecondTimer.Interval = 1000;
            vm.TweetsPerSecondTimer.Elapsed += (s, a) => {
                vm.TweetsPerSecond = 0.8f * vm.TweetsPerSecond + 0.2f * vm.TweetsLastSecond;
                vm.TweetsLastSecond = 0;
                vm.OnPropertyChanged("TweetsPerSecond");
            };
            vm.TweetsPerSecondTimer.Start();
            tweetsPerSecondLabel.DataContext = vm;



            // when credentials change
            vm.Credentials.CredentialsChange += (creds) => {
                Access_Token.Text = creds[0];
                Access_Token_Secret.Text = creds[1];
                Consumer_Key.Text = creds[2];
                Consumer_Secret.Text = creds[3];

            };


            // database output messages (not active if !databaseSaver.outputDatabaseMessages)
            vm.DatabaseSaver.Message += (s) => {
                vm.Log.Output(s);
            };


        }


        /// <summary>
        /// happens every sometimes, updates tweetView with the latest tweets (if so chosen)
        /// </summary>
        public void UpdateTweetView()
        {
            if (vm.UpdateTweetsNextUpdate) {
                vm.UpdateTweetsNextUpdate = false;
                App.Current.Dispatcher.Invoke(() => {
                    if (vm.ContinuousTweetViewUpdate) {
                        tweetView.ItemsSource = vm.TweetDatabase.Tweets;
                    }
                    vm.KeywordDatabase.KeywordList.UpdateCount(vm.TweetDatabase.GetAllTweets());
                });
            }
        }

        private void startStreamButton_Click(object sender, RoutedEventArgs e)
        {
            vm.Stream.Start();

        }

        private void restartStreamButton_Click(object sender, RoutedEventArgs e)
        {
            stopStreamButton_Click(sender, e);

            // wait until stream has stopped && streamRunning is false
            Task.Factory.StartNew(() => {
                vm.Log.Output("Separate thread waiting to start stream after it stops");
                while (vm.Stream.StreamRunning) {
                    // wait
                    ;
                }
                startStreamButton_Click(sender, e);
            });
        }

        private void stopStreamButton_Click(object sender, RoutedEventArgs e)
        {
            vm.Stream.Stop();

        }

        private void dMan_Unloaded(object sender, RoutedEventArgs e)
        {
            // save avalon layout to file
        }

        private void dMan_Loaded(object sender, RoutedEventArgs e)
        {
            // load avalon layout from file

            //credentialsPanelLayout.ToggleAutoHide();
            //streamingToolboxLayout.ToggleAutoHide();
            //logSettingsLayout.ToggleAutoHide();
        }


        private void restQueryButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.InvokeAsync((Action)(() => {

                string filter = restFilterTextBox.Text;

                // non-simple query never worked, might as well just have the simple query type
                bool simpleQuery = true;//((CheckBox)rest_filter_simpleQuery).IsChecked.Value;
                if (simpleQuery) {

                    vm.Log.Output("Getting Rest query with filter \"" + filter + "\"");

                    // perform Rest query using Rest class
                    var res = vm.Rest.SearchTweets(filter);

                    if (res == null) {
                        vm.Log.Output(("Not authenticated or invalid credentials"));
                        return;

                    }

                    if (res.Count == 0) {
                        vm.Log.Output(("No tweets returned"));
                    } else {
                        foreach (var r in res) {
                            vm.TweetDatabase.AddTweet(new TweetData(r, TweetData.Sources.Rest, 0, 0));
                        }
                        vm.UpdateTweetsNextUpdate = true;
                    }
                } else {
                    var recent = rest_filter_recent.IsChecked.Value;
                    var searchParameters = Tweetinvi.Search.CreateTweetSearchParameter(filter);
                    searchParameters.SearchType = recent ? Tweetinvi.Core.Enum.SearchResultType.Recent : Tweetinvi.Core.Enum.SearchResultType.Popular;

                    // display stuff in restInfoTextBlock.Text = "<here>"
                    vm.Log.Output(("Getting Rest query with advanced search parameters"));

                    Tweetinvi.Core.Exceptions.ITwitterException twitterException;
                    var res = vm.Rest.SearchTweets(searchParameters, out twitterException);

                    if (twitterException != null) {
                        vm.Log.Output("Latest exception from Tweetinvi! Status code: " + twitterException.StatusCode
                        + "\nException description: " + twitterException.TwitterDescription);
                    }

                    if (res == null) {
                        vm.Log.Output(("Not authenticated or invalid credentials"));
                        return;

                    }

                    if (res.Count == 0) {
                        vm.Log.Output(("No tweets returned"));
                    } else {
                        foreach (var r in res) {
                            vm.TweetDatabase.AddTweet(new TweetData(r, TweetData.Sources.Rest, 0, 0));
                        }
                        vm.UpdateTweetsNextUpdate = true;
                    }

                }
            }));

        }


        private void restExhaustiveQueryButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.Rest.IsGathering) {
                if (restStartDate.Value.HasValue && restEndDate.Value.HasValue) {
                    // start this in a new thread
                    //Task.Factory.StartNew(() => {
                    App.Current.Dispatcher.Invoke(() => {
                        vm.Rest.TweetsGatheringCycle(restStartDate.Value.Value, restEndDate.Value.Value, restFilterTextBox.Text.Split(',').ToList());
                    });
                    //});
                    restExhaustiveQueryButton.Content = "Stop Gathering Cycle";
                    restKeywordsQueryButton.Content = "Stop Gathering Cycle";

                } else {
                    // highlight startDate and endDate to signal the user that they need to be filled with values. #nicetohave
                    vm.Log.Output("Problem: Cannot start gathering cycle. Please set a *start date* and an *end date* for the query.");
                }
            } else {
                vm.Rest.StopGatheringCycle();
            }

            vm.Rest.StoppedGatheringCycle -= ResetRestGatheringButton;
            vm.Rest.StoppedGatheringCycle += ResetRestGatheringButton;
        }


        private void restKeywordsQueryButton_Click(object sender, RoutedEventArgs e)
        {
            // get keywords from keywordsList and use REST to do exhaustive query on them.
            if (!vm.Rest.IsGathering) {
                if (restStartDate.Value.HasValue && restEndDate.Value.HasValue) {
                    App.Current.Dispatcher.Invoke(() => {
                        vm.Rest.TweetsGatheringCycle(restStartDate.Value.Value, restEndDate.Value.Value, vm.KeywordDatabase.GetUsableKeywords());
                    });
                } else {
                    // highlight startDate and endDate to signal the user that they need to be filled with values. #nicetohave
                    vm.Log.Output("Problem: Cannot start gathering cycle. Please set a *start date* and an *end date* for the query.");
                }
                restExhaustiveQueryButton.Content = "Stop Gathering Cycle";
                restKeywordsQueryButton.Content = "Stop Gathering Cycle";

            } else {
                vm.Log.Output("Cannot start Rest gathering cycle because Rest gathering cycle in progress. Stopping now...");
                vm.Rest.StopGatheringCycle();
            }

            vm.Rest.StoppedGatheringCycle -= ResetRestGatheringButton;
            vm.Rest.StoppedGatheringCycle += ResetRestGatheringButton;
        }


        public void ResetRestGatheringButton(int tweetCount)
        {
            App.Current.Dispatcher.Invoke(() => {
                restExhaustiveQueryButton.Content = "Perform Exhaustive Query";
                restKeywordsQueryButton.Content = "Perform Query From KeywordList";

            });
        }



        private void SetTweetViewColumnHeaders()
        {
            // set column headers text to give info about stuff
            var cols = tweetViewGrid.Columns;
            // count languages?
            // count tweets?
            for (int i = 0; i < cols.Count; i++) {
                if (cols[i].Header.ToString().Contains("Tweet")) {
                    cols[i].Header = "Tweets (" + vm.TweetDatabase.Tweets.Count + ")";
                }
            }

        }



        private void SetKeywordListColumnHeaders()
        {
            // set column headers text to give info about stuff
            //var cols = restExpansionView.Columns;
            var cols = restExpansionListViewGrid.Columns;
            cols[0].Header = "Keywords (" + vm.KeywordDatabase.KeywordList.Count + ")";
            cols[1].Header = "Count";
            cols[2].Header = "Expansion ";
            for (int i = 0; i <= 2; i++) {
                cols[2].Header += vm.KeywordDatabase.KeywordList.Where(kd => kd.Expansion == i).Count().ToString() + " ";
            }
            // set up events for column headers for restExpansionView if they are not set up yet

        }



        private void restRateLimitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke((Action)(() => {
                // display stuff in restInfoTextBlock.Text = "<here>"
                vm.RestMessageList.Add(new LogMessage("Getting Rate Limit"));

                // gets rate limit using Rest class.
                var rl = vm.Rest.GetRateLimits_Search();

                if (rl == null) {
                    vm.RestMessageList.Add(new LogMessage("Not authenticated or invalid credentials (GetRateLimits_Search() was null)"));
                    return;

                }

                // readable info
                vm.RestMessageList.Add(new LogMessage("Limit: " + rl.Limit));
                vm.RestMessageList.Add(new LogMessage("Remaining: " + rl.Remaining));
                var time = rl.ResetDateTime.Subtract(DateTime.Now);
                vm.RestMessageList.Add(new LogMessage("Reset time: " + time.ToString(@"hh\:mm\:ss")));

            }));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.G) {
            //	bool g = panelLogAnchorablePane.CanClose;
            //	bool gg = panelLogAnchorable.CanClose;
            //	var a = 1;
            //}
        }

        private void setCredentialsButton_Click(object sender, RoutedEventArgs e)
        {
            //        credentials.TwitterCredentialsInit(new List<string>() {
            //// "Access_Token"
            //Access_Token.Text,
            //// "Access_Token_Secret"
            //Access_Token_Secret.Text,
            //// "Consumer_Key"
            //Consumer_Key.Text,
            //// "Consumer_Secret"
            //Consumer_Secret.Text
            //        });
            vm.Log.Output("Set custom credentials not implemented. Use the other button or command line arguments");
        }

        private void databasePhpPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (vm != null) {
                if (vm.DatabaseSaver != null) {
                    vm.DatabaseSaver.localPhpJsonLink = ((TextBox)sender).Text;
                    vm.LogMessageList.Add(new LogMessage("php path changed to " + vm.DatabaseSaver.localPhpJsonLink));
                }
            }
        }


        private void keywordListView_Headers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // order by kData
            if (vm.KeywordDatabase.KeywordList == null) return;
            if (vm.KeywordDatabase.KeywordList.Count == 0) return;
            if (((GridViewColumnHeader)sender).Content == null) return;

            App.Current.Dispatcher.Invoke(() => {
                if (((GridViewColumnHeader)sender).Content.ToString().Contains("Keyword")) {

                    vm.KeywordDatabase.KeywordList.Set(vm.KeywordDatabase.KeywordList.OrderBy(kd => kd.Keyword).ToList());

                } else if (((GridViewColumnHeader)sender).Content.ToString().Contains("Count")) {
                    // order by count
                    var maxCount = vm.KeywordDatabase.KeywordList.Max(kd => kd.Count);
                    if (vm.KeywordDatabase.KeywordList.FirstOrDefault().Count == maxCount) {
                        vm.KeywordDatabase.KeywordList.Set(vm.KeywordDatabase.KeywordList.OrderBy(kd => kd.Count).ToList());
                    } else {
                        vm.KeywordDatabase.KeywordList.Set(vm.KeywordDatabase.KeywordList.OrderByDescending(kd => kd.Count).ToList());
                    }
                } else {
                    // order by exp
                    var maxExp = vm.KeywordDatabase.KeywordList.Max(kd => kd.Expansion);
                    if (vm.KeywordDatabase.KeywordList.FirstOrDefault().Expansion == maxExp) {
                        vm.KeywordDatabase.KeywordList.Set(vm.KeywordDatabase.KeywordList.OrderBy(kd => kd.Expansion).ToList());
                    } else {
                        vm.KeywordDatabase.KeywordList.Set(vm.KeywordDatabase.KeywordList.OrderByDescending(kd => kd.Expansion).ToList());
                    }
                }
                keywordListView.UpdateLayout();
            });
        }

        private void keywordListView_Item_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (vm.KeywordDatabase.KeywordList == null) return;
            if (vm.KeywordDatabase.KeywordList.Count == 0) return;

            // get clicked row
            var item = ((ListViewItem)sender);
            var content = (KeywordData)(item.Content);

            // get kData
            // in tweet viewer, show only the tweets that contain said kData.
            App.Current.Dispatcher.Invoke((Action)(() => {
                vm.TweetDatabase.onlyShowKeywords.Clear();
                vm.TweetDatabase.onlyShowKeywords.Add(content.Keyword);

                // attempts refresh of tweetview
                tweetView.ItemsSource = vm.TweetDatabase.Tweets;
            }));

        }



        private void tweetView_Headers_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (vm.TweetDatabase.Tweets.Count == 0) return;

            App.Current.Dispatcher.Invoke((Action)(() => {
                var headerName = ((GridViewColumnHeader)sender).Content.ToString();
                List<TweetData> newList;
                if (headerName.Contains("Date")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.Date) == vm.TweetDatabase.Tweets.First().Date) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.Date).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.Date).ToList());
                    }

                } else if (headerName.Contains("Tweet")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.Tweet) == vm.TweetDatabase.Tweets.First().Tweet) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.Tweet).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.Tweet).ToList());
                    }
                } else if (headerName.Contains("UserId")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.UserId) == vm.TweetDatabase.Tweets.First().UserId) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.UserId).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.UserId).ToList());
                    }
                } else if (headerName.Contains("Id")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.Id) == vm.TweetDatabase.Tweets.First().Id) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.Id).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.Id).ToList());
                    }
                } else if (headerName.Contains("User")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.User) == vm.TweetDatabase.Tweets.First().User) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.User).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.User).ToList());
                    }
                } else if (headerName.Contains("Lang")) {
                    if (vm.TweetDatabase.Tweets.Min(t => t.Lang) == vm.TweetDatabase.Tweets.First().Lang) {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderByDescending(t => t.Lang).ToList());
                    } else {
                        newList = new List<TweetData>(vm.TweetDatabase.Tweets.OrderBy(t => t.Lang).ToList());
                    }
                } else {
                    newList = new List<TweetData>(vm.TweetDatabase.Tweets);
                }

                vm.TweetDatabase.Tweets.Clear();
                foreach (var n in newList) {
                    vm.TweetDatabase.Tweets.Add(n);

                }

                tweetView.UpdateLayout();
            }));
        }



        private void tweetView_Item_DeleteButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                var tweetToDelete = (TweetData)(item.Content);
                var result = MessageBox.Show("Delete tweet " + tweetToDelete.Tweet + " ?", "Delete confirmation", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes) {

                    App.Current.Dispatcher.Invoke(() => {
                        var hashtagsToUpdate = tweetToDelete.tweet.Hashtags.Select(hEntity => hEntity.Text.ToLower());
                        vm.TweetDatabase.RemoveTweet(tweetToDelete);
                        // update counts of hashtags
                        foreach (var k in vm.KeywordDatabase.KeywordList) {
                            if (hashtagsToUpdate.Contains(k.Keyword.Replace("#", "").ToLower())) {
                                k.Count = vm.TweetDatabase.GetAllTweets().Count(td => td.Tweet.ToLower().Contains(k.Keyword.ToLower()));
                            }
                        }

                        tweetView.ItemsSource = vm.TweetDatabase.Tweets;
                    });
                }
            }
        }

        private void keywordView_Item_DeleteButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                App.Current.Dispatcher.Invoke(() => {
                    var content = (KeywordData)(item.Content);
                    // delete kData from list
                    vm.KeywordDatabase.KeywordList.Remove(content);
                    // update header counts
                    SetKeywordListColumnHeaders();
                });
            }
        }

        private void keywordView_Item_StemButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                App.Current.Dispatcher.Invoke(() => {
                    var content = (KeywordData)(item.Content);
                    // stem word_i
                    var stemmed = vm.PorterStemmer.stemTerm(content.Keyword);
                    content.Keyword = stemmed;

                });
            }
        }

        private T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
                return null;

            if (parent is T)
                return parent as T;
            else
                return FindParent<T>(parent);
        }



        private void tweetView_ResetSelection(object sender, RoutedEventArgs e)
        {
            var message = "";
            if (vm.TweetDatabase.GetAllTweets().Count > 50000) {
                message = "It WILL take forever to load " + vm.TweetDatabase.GetAllTweets().Count + " tweets. Please reconsider or continue at your own risk.";
            } else if (vm.TweetDatabase.GetAllTweets().Count > 10000) {
                message = "It might take quite a while to load " + vm.TweetDatabase.GetAllTweets().Count + " tweets. Please have patience";
            } else {
                message = "";
            }

            MessageBoxResult m = MessageBoxResult.None;
            if (message != "") {
                m = MessageBox.Show("Are you sure you want to reset selection? " + message, "Warning", MessageBoxButton.YesNo);
            }

            if ((message != "" && m == MessageBoxResult.Yes) || message == "") {
                App.Current.Dispatcher.Invoke(() => {
                    vm.TweetDatabase.onlyShowKeywords.Clear();

                    // attempts refresh of tweetview
                    tweetView.ItemsSource = vm.TweetDatabase.Tweets;

                    keywordListView.UnselectAll();
                });
            }
        }

        private void tweetView_DeleteAll(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all tweets? The database and text files will not be affected.", "Delete confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                App.Current.Dispatcher.Invoke(() => {
                    vm.TweetDatabase.RemoveAllTweets();
                    foreach (var k in vm.KeywordDatabase.KeywordList) {
                        k.Count = 0;

                    }
                    keywordListView.ItemsSource = vm.KeywordDatabase.KeywordList;
                });
            }
        }

        private void logClearButtonClick(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                vm.LogMessageList.Clear();
            });
            logView.UpdateLayout();
        }

        private void keywordsView_ResetSelection(object sender, RoutedEventArgs e)
        {
            tweetView_ResetSelection(sender, e);
        }

        private void keywordsView_DeleteAll(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete all keywords?", "Delete confirmation", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes) {
                App.Current.Dispatcher.Invoke(() => {
                    vm.KeywordDatabase.KeywordList.Clear();

                    SetKeywordListColumnHeaders();
                });
            }
        }


        private void keywordAddButtonClick(object sender, RoutedEventArgs e)
        {
            var newKeyword = keywordAddTextbox.Text;
            //newKeyword = newKeyword.Replace(" ", "");

            if (newKeyword != "") {
                //if (!(newKeyword[0] == '#')) {
                //	newKeyword = newKeyword.Insert(0, "#");
                //}

                var newKeywordData = new KeywordData(newKeyword, 0);
                newKeywordData.Count = vm.TweetDatabase.GetAllTweets().Count(t => t.ContainsHashtag(newKeyword));

                App.Current.Dispatcher.Invoke(() => {
                    vm.KeywordDatabase.KeywordList.Add(newKeywordData);

                    SetKeywordListColumnHeaders();

                    keywordAddTextbox.Text = "";
                });
            }
        }


        private void keywordsView_UseAll(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                foreach (var kd in vm.KeywordDatabase.KeywordList) {
                    kd.UseKeyword = true;
                }

                //keywordDatabase.KeywordList.Set(newKeys);

                keywordListView.UpdateLayout();
            });
        }
        private void keywordsView_UseNone(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                foreach (var kd in vm.KeywordDatabase.KeywordList) {
                    kd.UseKeyword = false;
                }

                //keywordDatabase.KeywordList.Set(newKeys);

                keywordListView.UpdateLayout();
            });
        }

        private void expansion_ExpandNaive(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => {
                var newKeywords = vm.QueryExpansion.ExpandNaive(vm.KeywordDatabase.KeywordList.ToList(), vm.TweetDatabase.GetAllTweets().ToList());
                //keywordDatabase.KeywordList.UpdateCount(tweetDatabase.tweets);

                // COUNT IS ALREADY PERFORMED IN THE NAIVE EXPANSION!!!

                // update count outside of main thread
                //foreach (var k in newKeywords) {
                //	k.Count = tweetDatabase.tweets.Count(td => td.ContainsHashtag(k.Keyword));
                //}

                if (newKeywords != null && newKeywords.Count > 0) {
                    App.Current.Dispatcher.Invoke(() => {
                        vm.KeywordDatabase.KeywordList.AddRange(newKeywords);
                    });
                }
            });
        }


        public void AutoExpand()
        {
            if (vm.Stream.StreamRunning) {
                vm.Log.Output("Attempting to expand the query automatically");
                Task.Factory.StartNew(() => {
                    // before Efron, more tags must be gathered. preferably a full transitive closure
                    var newKeys = vm.QueryExpansion.ExpandNaive(vm.KeywordDatabase.KeywordList.ToList(), vm.TweetDatabase.GetAllTweets());
                    if (newKeys != null && newKeys.Count > 0) {
                        App.Current.Dispatcher.Invoke(() => {
                            // set use to false (we might not want them in the stream query)
                            newKeys.ForEach(kd => kd.UseKeyword = false);
                            // add the new keywords
                            vm.KeywordDatabase.KeywordList.AddRange(newKeys);

                            // clear language models, every time.
                            vm.KeywordDatabase.KeywordList.ClearLanguageModels();

                            // now we can expand.
                            var efronExpanded = vm.QueryExpansion.ExpandEfron(vm.KeywordDatabase.KeywordList, vm.TweetDatabase.GetAllTweets());

                            // we must generate the query from the query model.
                            vm.KeywordDatabase.KeywordList.Where(kd => efronExpanded.Any(kkk => kkk.Keyword == kd.Keyword)).ToList().ForEach(kd => kd.UseKeyword = true);

                            // after expansion, restart stream to apply the new settings.
                            vm.Log.Output("Attempting to restart the stream automatically");
                            restartStreamButton_Click(null, null);
                        });
                    }
                });
            }
        }

        private void expansion_ExpandEfron(object sender, RoutedEventArgs e)
        {
            if (!vm.QueryExpansion.Expanding) {
                Task.Factory.StartNew(() => {
                    vm.QueryExpansion.ExpandEfron(vm.KeywordDatabase.KeywordList, vm.TweetDatabase.GetAllTweets());

                    vm.QueryExpansion.Expanding = false;
                });
            } else {
                var m = MessageBox.Show("Expansion running. Cancel current expansion?", "wait what?", MessageBoxButton.YesNo);
                if (m == MessageBoxResult.Yes) {
                    vm.QueryExpansion.Stop();
                }
            }
        }

        private void setCredentialsDefault_Click(object sender, RoutedEventArgs e)
        {
            vm.CurrentCredentialDefaults++;
            vm.Credentials.SetCredentials(vm.CurrentCredentialDefaults);
        }


        private void tweetView_LoadFromFile(object sender, RoutedEventArgs e)
        {
            if (!vm.FromFile_isLoading) {

                ((Button)sender).Content = "Cancel operation";

                vm.FromFile_Loaded = false;
                vm.FromFile_isLoading = true;
                // start task which updates UI
                vm.FromFile_updateUiTimer = new System.Timers.Timer(100);
                vm.FromFile_updateUiTimer.Elapsed += (s, a) => {
                    vm.OnPropertyChanged("FromFileLoader");
                    if (vm.FromFile_Loaded) {
                        fromFile_percentDone = vm.FromFile_tweetLoadedCount / vm.FromFile_tweetCount;

                    }

                    if (!vm.FromFile_isLoading) {
                        vm.FromFile_updateUiTimer.Stop();
                        vm.FromFile_updateUiTimer.Dispose();
                    }
                };
                vm.FromFile_updateUiTimer.Start();

                Task.Factory.StartNew(() => {
                    var fromTxt = vm.DatabaseSaver.LoadFromTextFile(vm.DatabaseSaver.TextFileDatabasePath, ref fromFile_percentDone);
                    vm.Log.Output("Loaded files. Dumping them into tweetList");
                    vm.FromFile_Loaded = true;
                    vm.FromFile_tweetCount = fromTxt.Count;
                    vm.FromFile_tweetLoadedCount = 0;

                    // add in bulk
                    if (false) {
                        while (fromTxt.Count > 0) {
                            var smallList = fromTxt.GetRange(Math.Max(fromTxt.Count - 1000, 0), Math.Min(1000, fromTxt.Count));
                            for (int i = 0; i < smallList.Count; i++) {
                                vm.TweetDatabase.AddTweet(smallList[i]);
                            }
                            fromTxt.RemoveRange(Math.Max(fromTxt.Count - 1000, 0), Math.Min(1000, fromTxt.Count));
                            vm.FromFile_tweetLoadedCount += 1000;
                        }
                    }
                    // add one by one
                    var checkEachTweet = false;
                    if (checkEachTweet) {
                        for (int i = 0; i < fromTxt.Count; i++) {
                            vm.TweetDatabase.AddTweet(fromTxt[i]);
                            vm.FromFile_tweetLoadedCount++;
                            if (vm.DatabaseSaver.fromFile_cancelOperation) {
                                vm.DatabaseSaver.fromFile_cancelOperation = false;
                                break;
                            }
                        }
                    } else {
                        vm.TweetDatabase.AddTweets(fromTxt);
                        vm.FromFile_tweetLoadedCount = fromTxt.Count;
                    }

                    App.Current.Dispatcher.Invoke(() => {
                        ((Button)sender).Content = "Load from text file";
                    });
                    vm.Log.Output("Finished loading " + vm.FromFile_tweetLoadedCount + " tweets from file");
                    vm.FromFile_isLoading = false;

                });

            } else {
                vm.DatabaseSaver.fromFile_cancelOperation = true;

            }

        }


        private void keywordsView_Update(object sender, RoutedEventArgs e)
        {
            // counts all keywords
            vm.KeywordDatabase.KeywordList.UpdateCount(vm.TweetDatabase.GetAllTweets());

            // update list view
            keywordListView.UpdateLayout();
        }


        private void tweetView_Item_FollowLink(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                var content = (TweetData)(item.Content);
                try {
                    System.Diagnostics.Process.Start("https://twitter.com/search?q=" + content.Id);
                }
                catch {
                    MessageBox.Show("Something went wrong when trying to open link " + "https://twitter.com/search?q=" + content.Id + "\n\n Try again maybe");
                }
            }
        }


        private void tweetView_KeepTweets(object sender, RoutedEventArgs e)
        {
            int howMany;
            if (int.TryParse(tweetView_keepHowManyTweets_textBox.Text, out howMany)) {
                vm.TweetDatabase.KeepTweets(howMany);
                vm.Log.Output("Kept only the first " + vm.TweetDatabase.GetAllTweets().Count + " tweets");
                vm.UpdateTweetsNextUpdate = true;
            } else if (tweetView_keepHowManyTweets_dateBox.Value.HasValue) {
                var date = tweetView_keepHowManyTweets_dateBox.Value.Value;
                vm.TweetDatabase.KeepTweetsBeforeDate(date);
                vm.Log.Output("Deleted tweets after date " + date);
                vm.UpdateTweetsNextUpdate = true;
            }
        }

        private void keywordView_Item_LangModelCalculateButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                App.Current.Dispatcher.Invoke(() => {
                    var content = (KeywordData)(item.Content);
                    if (!content.HasLanguageModel) {
                        content.LanguageModel = new LanguageModel(content, vm.KeywordDatabase.KeywordList, vm.TweetDatabase.GetAllTweets(), null, LanguageModel.SmoothingMethods.BayesianDirichlet, vm.QueryExpansion.EfronMu);
                    } else {
                        vm.Log.Output("Keyword " + content.Keyword + " already has a calculated language model");
                        vm.Log.Output(content.LanguageModel.ToString());
                    }
                });
            }
        }

        private void keywordView_Item_LangModelClearButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                App.Current.Dispatcher.Invoke(() => {
                    var content = (KeywordData)(item.Content);
                    content.LanguageModel = null;
                });
            }
        }

        private void keywordView_Item_LangModelPrintButton(object sender, RoutedEventArgs e)
        {
            // get clicked row
            var button = ((Button)sender);
            var item = FindParent<ListViewItem>(button);
            if (item != null) {
                App.Current.Dispatcher.Invoke(() => {
                    var content = (KeywordData)(item.Content);

                    if (content.HasLanguageModel) {
                        vm.Log.Output(content.LanguageModel.ToString());
                    } else {
                        vm.Log.Output(content.Keyword + " does not have a language model");
                    }
                });
            }
        }


        private void tweetView_DeleteSpamTweets(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                vm.TweetDatabase.RemoveAllRT();
                vm.Log.Output("Removed tweets starting with RT, now we have " + vm.TweetDatabase.GetAllTweets().Count + " tweets");
            });
        }

        private void keywordsView_DeleteSelected(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to delete selected keywords?", "Delete confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {

            }
        }

        private void keywordsView_ClearLangModels(object sender, RoutedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => {
                var i = 0;
                while (i < vm.KeywordDatabase.KeywordList.Count) {
                    vm.KeywordDatabase.KeywordList[i].LanguageModel = null;
                    i++;
                }

            });
        }

        private void log_forceResetButton(object sender, RoutedEventArgs e)
        {
            vm.Log.Output("Log force reset no longer supported");
        }


    }

}
