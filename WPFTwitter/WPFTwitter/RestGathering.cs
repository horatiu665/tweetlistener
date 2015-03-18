using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces.Models.Parameters;

namespace WPFTwitter
{
	/// <summary>
	/// used to gather tweets and expand the query to include all related hashtags/keywords (customizable) 
	/// until a certain generation (number of iterations from original set of keywords)
	/// </summary>
	public class RestGatherer
	{
		private bool stop = false;
		public void Stop()
		{
			stop = true;
		}

		/// <summary>
		/// stores a list of the current keywords being looked for
		/// </summary>
		private List<KeywordData> keywordList;
		public ObservableCollection<KeywordData> KeywordList
		{
			get {
				return new ObservableCollection<KeywordData>(keywordList);
			}
		}

		private List<TweetData> tweetsProcessed;

		/// <summary>
		/// how many times did we expand the query? 
		/// </summary>
		private ExpansionData currentExpansion;

		// how many times to expand query in each cycle?
		private int maxExpansionCount = 3;

		/// <summary>
		/// how many times did the gatherer finish a sequence of expansion and restarted for a new set of dates?
		/// </summary>
		private int gatheringCycle;

		/// <summary>
		/// datetime when rate limit will reset to max value
		/// </summary>
		private DateTime rateLimitReset;

		/// <summary>
		/// TODO: reset this counter at the right times.
		/// can query until this is <= 0
		/// </summary>
		private int rateLimitCounter;

		/// <summary>
		/// get tweets since this date up until present day. default = present day.
		/// </summary>
		private DateTime sinceDate;

		/// <summary>
		/// get tweets until this date, to not combine/confuse the repeated sessions of gathering.
		/// after all tweets were gathered once until generation X, start gathering again, change sinceDate to untilDate and untilDate to present date.
		/// </summary>
		private DateTime untilDate;

		public event EventHandler<TweetData> TweetFound;
		public event EventHandler<ExpansionData> ExpansionFinished;
		public event EventHandler<DateTime> CycleFinished;
		public event EventHandler Stopped;

		public RestGatherer()
		{
			Reset();

		}

		/// <summary>
		/// clear all.
		/// </summary>
		public void Reset()
		{
			keywordList = new List<KeywordData>();
			currentExpansion = new ExpansionData(0);
			gatheringCycle = 0;
			// ratelimit counter depends on twitter, so reset will be basically polling twitter for the data.
			var rl = Rest.GetRateLimits_Search();
			rateLimitCounter = rl.Remaining;
			rateLimitReset = rl.ResetDateTime;
			sinceDate = DateTime.Now;
			untilDate = DateTime.Now;
			tweetsProcessed = new List<TweetData>();
		}

		/// <summary>
		/// should be started in new thread.
		/// </summary>
		public void Algorithm(int expansions, params string[] filters)
		{
			var startdate = DateTime.Now;
			startdate.AddDays(-7);

			AddKeywords(0, filters);

			TweetFound += (s, t) => {
				ProcessTweet(t);
				currentExpansion.tweets.Add(t);
				
			};

			Task.Factory.StartNew(() => {
				while (!stop)
					GatheringCycle(startdate, DateTime.Now);
			});
		}

		/// <summary>
		/// perform one gathering cycle until all tweets and expanded versions were gathered, and call new gathering cycle from untilDate until present.
		/// </summary>
		private void GatheringCycle(DateTime sinceDate, DateTime untilDate)
		{
			this.sinceDate = sinceDate;
			this.untilDate = untilDate;

			for (var i = 0; i < maxExpansionCount; i++) {
				currentExpansion = new ExpansionData(i);
				
				var sp = ResetSearchParameters();

				var results = new List<ITweet>();

				// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
				do {
					if (rateLimitCounter > 0 && !stop) {
						results = Rest.SearchTweets(sp);
						if (results.Count > 0) {

							rateLimitCounter--;

							// save minId to use for next batch of tweets
							var minId = results[0].IdStr;
							foreach (var r in results) {
								// find minId among results.
								if (stringIntSmallerThan(r.IdStr, minId)) {
									minId = r.IdStr;
								}

								TweetFound(this, new TweetData(r, gatheringCycle, currentExpansion.expansion));

							}

							sp.MaxId = int.Parse(minId);
						} else {
							break;
						}
					}
				} while (results.Count > 0 || rateLimitCounter <= 0 || stop);

				// expansion finished
				ExpansionFinished(this, currentExpansion);
			}

			// all tweets were gathered in the tweetsToProcess list, which is processed separately by another process (pun)

			// the query should be expanded by the other process, and the gathering cycle restarted with settings:
			//			RestartGatheringCycle(untilDate, DateTime.Now);
			CycleFinished(this, untilDate);
		}

		// expands keywords, saves tweets. runs in BG, processes tweets when it has enough resources.
		private void ProcessTweet(TweetData t)
		{
			AddKeywords(currentExpansion.expansion, t.GetHashtags().ToArray());
			tweetsProcessed.Add(t);
			
		}


		/// <summary>
		/// adds list of keywords at current generation.
		/// </summary>
		/// <param name="keywords"></param>
		private void AddKeywords(int expansion, params string[] keywords)
		{
			foreach (var k in keywords) {
				keywordList.Add(new KeywordData(k, expansion));
			}
		}

		/// <summary>
		/// compares two ints made into strings because they are too long (tweetId-s)
		/// </summary>
		/// <returns>true when int1 < int2</returns>
		private bool stringIntSmallerThan(string int1, string int2)
		{
			if (int1.Length > int2.Length) {
				return false;
			} else if (int2.Length < int1.Length) {
				return true;
			} else {
				return (string.Compare(int1, int2) == -1);
			}
		}



		/// <summary>
		/// returns search parameters based on keywords in current expansion,
		/// and since/until dates. also maxId because we try to get all tweets and no repeats	
		/// </summary>
		private ITweetSearchParameters ResetSearchParameters()
		{
			// get all tweets using keyword list
			// set the latest keywords as search query
			var sp = Rest.GenerateSearchParameters("");
			foreach (var k in keywordList) {
				if (k.expansion == currentExpansion.expansion) {
					sp.SearchQuery += k.keyword + ", ";
				}
			}
			sp.SearchType = SearchResultType.Recent;
			sp.TweetSearchFilter = TweetSearchFilter.All;

			sp.Since = sinceDate;

			sp.Until = untilDate;

			sp.MaximumNumberOfResults = 100;

			sp.MaxId = long.MaxValue;

			return sp;

		}

		public class ExpansionData
		{
			public int expansion;
			
			public int TweetCount
			{
				get { return tweets != null ? tweets.Count : 0; }
			}

			public List<TweetData> tweets;

			public ExpansionData(int num)
			{
				expansion = num;
			}

		}

		public class TweetData
		{
			// tweet data
			public ITweet tweet;

			// which cycle yielded this tweet?
			public int gatheringCycle;

			// which expansion cycle first found this tweet?
			public int firstExpansion;

			public TweetData(ITweet tweet, int gatheringCycle, int firstExpansion)
			{
				this.tweet = tweet;
				this.gatheringCycle = gatheringCycle;
				this.firstExpansion = firstExpansion;

			}

			// returns a list of hashtags.
			public List<string> GetHashtags()
			{
				return tweet.Entities.Hashtags.Select(h => h.Text).ToList();
			}

		}

		// keywords are added by expanding the original keywords, and each expansion is expanding the next expansion etc. until maxExpansionCount
		public class KeywordData
		{
			/// <summary>
			/// the actual hashtag/keyword string.
			/// </summary>
			public string keyword;

			/// <summary>
			/// how many times since the original tweet were tweets with similar keywords pulled?
			/// </summary>
			public int expansion;

			public KeywordData(string keyword, int generation)
			{
				this.keyword = keyword;
				this.expansion = generation;

			}

		}

	}

}
