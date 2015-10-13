using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{
    public class TweetDatabase
    {
        Database db;

        private bool saveToRamProperty = true;
        public bool SaveToRamProperty
        {
            get { return saveToRamProperty; }
            set { saveToRamProperty = value; }
        }

        public TweetDatabase(Database databaseSaver)
        {
            db = databaseSaver;

            // initialize mostrecenttweet with datetime.now because otherwise any query will spend 
            // a few days gathering all the known previous tweets (which might not be such a bad 
            // idea but still is a bit time and energy consuming - must test).
            //mostRecentTweet = DateTime.Now;
        }

        public bool OnlyEnglish { get; set; }
        public bool OnlyWithHashtags { get; set; }

        /// <summary>
        /// store the tweet database here
        /// </summary>
        private List<TweetData> tweets = new List<TweetData>();

        /// <summary>
        /// get all tweets
        /// </summary>
        /// <returns></returns>
        public List<TweetData> GetAllTweets()
        {
            return tweets;
        }

        public void RemoveTweet(TweetData tweet)
        {
            tweets.Remove(tweet);
        }

        public void RemoveAllTweets()
        {
            tweets.Clear();
        }

        /// <summary>
        /// adds tweets, but does not verify if they are duplicates and such
        /// </summary>
        /// <param name="newTweets"></param>
        public void AddTweets(List<TweetData> newTweets)
        {
            if (SaveToRamProperty) {
                tweets.AddRange(newTweets);
            }

            mostRecentTweet = DateTime.Now;
        }

        /// <summary>
        /// only keep first N tweets
        /// </summary>
        /// <param name="howMany"></param>
        public void KeepTweets(int howMany)
        {
            var newList = tweets.Take(howMany).ToList();
            tweets.Clear();
            tweets.AddRange(newList);

        }

        /// <summary>
        /// only keep tweets published before date
        /// </summary>
        /// <param name="date"></param>
        public void KeepTweetsBeforeDate(DateTime date)
        {
            var newList = tweets.Where(td => td.Date.CompareTo(date) <= 0).ToList();
            tweets.Clear();
            tweets.AddRange(newList);
        }

        /// <summary>
        /// remove all retweets (tweets which start with RT). 
        /// </summary>
        public void RemoveAllRT()
        {
            tweets.RemoveAll(td => td.Tweet.IndexOf("RT") == 0);
        }

        /// <summary>
        /// getter with filter. take from tweets, and only show the ones we want to show based on onlyShowKeywords
        /// </summary>
        public RangeObservableCollection<TweetData> Tweets
        {
            get
            {
                RangeObservableCollection<TweetData> show = new RangeObservableCollection<TweetData>();

                try {
                    if (onlyShowKeywords.Count > 0) {
                        var showList = tweets.Where(td => {
                            foreach (var k in onlyShowKeywords) {
                                if (td.Tweet.ToLower().Contains(k.ToLower()))
                                    return true;
                            }
                            return false;
                        });
                        show.AddRange(showList);
                        return show;
                    }

                    show.AddRange(tweets);
                }
                catch {

                }
                return show;
            }
        }

        public List<string> onlyShowKeywords = new List<string>();

        /// <summary>
        /// did the stream stop previously? when?
        /// </summary>
        private DateTime mostRecentTweet = new DateTime(1993, 7, 23);

        public DateTime MostRecentTweetTime
        {
            get { return mostRecentTweet; }
        }

        public void AddTweet(TweetData item)
        {
            if (OnlyEnglish) {
                if (item.tweet.Language != Tweetinvi.Core.Enum.Language.English)
                    if (item.tweet.Language != Tweetinvi.Core.Enum.Language.UN_NotReferenced)
                        if (item.tweet.Language != Tweetinvi.Core.Enum.Language.Undefined)
                            return;
            }

            if (OnlyWithHashtags) {
                if (item.tweet.Hashtags.Count == 0)
                    return;
            }

            // only add tweet if it has not been found before (REST can find same tweets again)
            if (tweets.Any(td => td.Id == item.Id)) {
                // update retweet count but nothing more than that.
                tweets.First(td => td.Id == item.Id).RetweetCount = item.RetweetCount;
                return;
            }

            // if tweet contains links, it is possible that it is a clone of another tweet, but with a new link. (tweetbots)


            // if we captured a non-retweet, add it to the list.
            if (!item.tweet.IsRetweet) {
                if (SaveToRamProperty) {
                    tweets.Add(item);
                }

                db.SaveTweet(item.tweet);
            } else {
                // if there is an item already with the ID equal to the retweet, increase count
                if (tweets.Any(td => td.Id == item.tweet.RetweetedTweet.Id)) {
                    tweets.First(td => td.Id == item.tweet.RetweetedTweet.Id).RetweetCount++;
                } else {
                    // add the original tweet to the list instead of the retweet.
                    if (SaveToRamProperty) {
                        tweets.Add(new TweetData(item.tweet.RetweetedTweet, item.source, item.gatheringCycle, item.firstExpansion));
                    }
                    db.SaveTweet(item.tweet.RetweetedTweet);
                }
            }

            // tweet added successfully. update date of last tweet.
            mostRecentTweet = DateTime.Now;
        }
        

    }

}
