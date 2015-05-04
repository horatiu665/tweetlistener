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
		private ObservableCollection<TweetData> tweets = new ObservableCollection<TweetData>();

		public ObservableCollection<TweetData> Tweets
		{
			get { return tweets; }
			set { tweets = value; }
		}


		public class TweetData
		{
			public ITweet tweet;

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
		}
	}
}
