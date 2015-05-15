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
		private ObservableCollection<TweetData> tweets = new ObservableCollection<TweetData>();

		/// <summary>
		/// take from tweets, and only show the ones we want to show based on onlyShowKeywords
		/// </summary>
		public ObservableCollection<TweetData> Tweets
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
					ObservableCollection<TweetData> show = new ObservableCollection<TweetData>(showList);
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

		public ObservableCollection<TweetData> AllTweets
		{
			get
			{
				return tweets;
			}
		}

		public List<string> onlyShowKeywords = new List<string>();


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

		}
	}
}
