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
		/// store all tweets here
		/// </summary>
		//private ObservableCollection<TweetData> realTweets = new ObservableCollection<TweetData>();

		/// <summary>
		/// store what we show here
		/// </summary>
		private ObservableCollection<TweetData> tweets = new ObservableCollection<TweetData>();

		/// <summary>
		/// when accessing, take from realTweets and save to tweets, and only show the ones we want to show.
		/// </summary>
		public ObservableCollection<TweetData> Tweets
		{
			get
			{
				if (onlyShowKeywords.Count > 0) {
					var showList = tweets.Where(td => {
						foreach (var k in onlyShowKeywords) {
							if (td.Tweet.Contains(k))
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

		public List<string> onlyShowKeywords = new List<string>();


		public class TweetData
		{
			public ITweet tweet;

			public TweetData(ITweet t)
			{
				tweet = t;

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
