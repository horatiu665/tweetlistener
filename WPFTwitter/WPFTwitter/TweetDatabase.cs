using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;
using System.Collections.ObjectModel;


namespace WPFTwitter
{
	public class TweetDatabase
	{

		/// <summary>
		/// store the tweet database here
		/// </summary>
		private TweetList tweets = new TweetList();

		/// <summary>
		/// take from tweets, and only show the ones we want to show based on onlyShowKeywords
		/// </summary>
		public TweetList Tweets
		{
			get
			{
				if (onlyShowKeywords.Count > 0) {
					var showList = tweets.Where(td => {
						foreach (var k in onlyShowKeywords) {
							if (td.Tweet.ToLower().Contains(k.ToLower()))
								return true;
						}
						return false;
					});
					TweetList show = (TweetList) new ObservableCollection<TweetData>(showList);
					return show;
				}

				return tweets;
			}
			set
			{
				tweets.Clear();
				foreach (var k in value) {
					tweets.Add(k);
				}

			}
		}

		public TweetList AllTweets
		{
			get
			{
				return tweets;
			}
		}

		public List<string> onlyShowKeywords = new List<string>();

		public class TweetList : ObservableCollection<TweetData>
		{
<<<<<<< HEAD
=======
			
>>>>>>> origin/wiki

			protected override void InsertItem(int index, TweetData item)
			{
				if (item.tweet.IsRetweet && this.Any(td => td.Id == item.tweet.RetweetedTweet.Id)) {
					this.First(td => td.Id == item.tweet.RetweetedTweet.Id).RetweetCount++;
				} else {
					base.InsertItem(index, item);
				}
			}

		}

		public class TweetData
		{
			public ITweet tweet;

			public enum Sources
			{
				Stream,
				Rest
			}

			// where did we find this tweet?
			public Sources source;

			// which cycle yielded this tweet?
			public int gatheringCycle;

			// which _expansion cycle first found this tweet?
			public int firstExpansion;

			// how many times was this tweet found after being found once and processed?
			public int howManyTimesFound = 1;

<<<<<<< HEAD
=======
			/// <summary>
			/// returns list of hashtags as found raw in ITweet.
			/// </summary>
			/// <returns></returns>
>>>>>>> origin/wiki
			public List<string> GetHashtags()
			{
				return tweet.Hashtags.Select(he => he.Text).ToList();
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
				}
			}

		}
	}
}
