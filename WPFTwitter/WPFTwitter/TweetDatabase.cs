using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections.Specialized;


namespace WPFTwitter
{
	public class TweetDatabase
	{
		DatabaseSaver db;

		private bool saveToRamProperty = true;
		public bool SaveToRamProperty
		{
			get { return saveToRamProperty; }
			set { saveToRamProperty = value; }
		}

		public TweetDatabase(DatabaseSaver databaseSaver)
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
		private TweetList tweets = new TweetList();

		/// <summary>
		/// get all tweets
		/// </summary>
		/// <returns></returns>
		public TweetList GetAllTweets()
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

		public class TweetList : List<TweetData>
		{

		}

		public class TweetData : INotifyPropertyChanged
		{
			public ITweet tweet;

			public enum Sources
			{
				Stream,
				Rest,
				Unknown
			}

			// where did we find this tweet?
			public Sources source;

			// which cycle yielded this tweet?
			public int gatheringCycle;

			// which _expansion cycle first found this tweet?
			public int firstExpansion;

			// how many times was this tweet found after being found once and processed?
			public int howManyTimesFound = 1;

			/// <summary>
			/// returns list of hashtags as found raw in ITweet.
			/// </summary>
			/// <returns></returns>
			public List<string> GetHashtags()
			{
				return tweet.Hashtags.Select(he => he.Text).ToList();
			}

			/// <summary>
			/// does this tweet contain the hashtag tag?
			/// </summary>
			/// <param name="tag">hashtag to contain</param>
			/// <returns></returns>
			public bool ContainsHashtag(string tag)
			{
				if (tweet.Hashtags.Count == 0) return false;
				var withoutHash = tag.IndexOf("#") == 0 ? tag.Substring(1) : tag;
				return tweet.Hashtags.Select(he => he.Text.ToLower()).Contains(withoutHash);
			}

			public TweetData(ITweet t, Sources source, int gatheringCycle, int firstExpansion)
			{
				tweet = t;
				this.source = source;
				this.gatheringCycle = gatheringCycle;
				this.firstExpansion = firstExpansion;

				this.retweetCount = t.RetweetCount;

			}

			public DateTime Date
			{
				get
				{
					return tweet.CreatedAt;
				}
			}

			public string Tweet
			{
				get
				{
					return tweet.Text;
				}
			}

			public long Id
			{
				get
				{
					return tweet.Id;
				}
			}

			public string Lang
			{
				get
				{
					return tweet.Language.ToString();
				}
			}

			public string User
			{
				get
				{
					return tweet.Creator.Name;
				}
			}

			public long UserId
			{
				get
				{
					return tweet.Creator.Id;
				}
			}

			private int retweetCount = 0;
			/// <summary>
			/// counts how many times the tweet was posted again with a different link 
			/// (most likely by a tweet bot)
			/// </summary>
			private int duplicateCount = 0;


			/// <summary>
			/// Retweet count, counted manually
			/// beginning at twitter data when TweetData is constructed
			/// and counting every time the tweet id is found in another tweet's retweet id
			/// </summary>
			public int RetweetCount
			{
				get
				{
					return retweetCount;
				}
				set
				{
					retweetCount = value;
					//if (PropertyChanged != null) {
					//	PropertyChanged(this, new PropertyChangedEventArgs("RetweetCount"));
					//}
				}
			}

			public List<string> Links
			{
				get
				{
					return tweet.Urls.Select(urlEntity => urlEntity.URL).ToList();
				}
			}


			public event PropertyChangedEventHandler PropertyChanged;
		}
	}


	/// <summary>
	/// clever overloading and avoiding update of the UI before all items in a range have been loaded. LIFESAVER!!!
	/// https://peteohanlon.wordpress.com/2008/10/22/bulk-loading-in-observablecollection/
	/// </summary>
	public class RangeObservableCollection<T> : ObservableCollection<T>
	{
		private bool _suppressNotification = false;

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (!_suppressNotification)
				base.OnCollectionChanged(e);
		}

		public void AddRange(IEnumerable<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			_suppressNotification = true;

			foreach (T item in list) {
				this.Add(item);
			}

			_suppressNotification = false;
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}
	}

}
