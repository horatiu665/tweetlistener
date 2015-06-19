﻿using System;
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

		/// <summary>
		/// store the tweet database here
		/// </summary>
		public List<TweetData> tweets = new List<TweetData>();


		/// <summary>
		/// take from tweets, and only show the ones we want to show based on onlyShowKeywords
		/// </summary>
		public TweetList Tweets
		{
			get
			{
				TweetList show = new TweetList();

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
				return show;
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
				var show = new TweetList();
				show.AddRange(tweets);
				return show;
			}
		}

		public List<string> onlyShowKeywords = new List<string>();

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


		public class TweetList : RangeObservableCollection<TweetData>
		{
			protected override void InsertItem(int index, TweetData item)
			{
				// only add tweet if it has not been found before (REST can find same tweets again)
				if (this.Any(td => td.Id == item.Id)) {
					// update retweet count but nothing more than that.
					this.First(td => td.Id == item.Id).RetweetCount = item.RetweetCount;
					return;
				}

				// if tweet contains links, it is possible that it is a clone of another tweet, but with a new link. (tweetbots)


				// if we captured a non-retweet, add it to the list.
				if (!item.tweet.IsRetweet) {
					base.InsertItem(index, item);
				} else {
					// if there is an item already with the ID equal to the retweet, increase count
					if (this.Any(td => td.Id == item.tweet.RetweetedTweet.Id)) {
						this.First(td => td.Id == item.tweet.RetweetedTweet.Id).RetweetCount++;
					} else {
						// add the original tweet to the list instead of the retweet.
						base.InsertItem(index, new TweetData(item.tweet.RetweetedTweet, item.source, item.gatheringCycle, item.firstExpansion));
					}
				}
			}

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
}
