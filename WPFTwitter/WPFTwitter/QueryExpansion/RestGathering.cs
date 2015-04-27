﻿using System;
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

		public class KeywordListClass : ObservableCollection<KeywordData>
		{
			public void Set(ObservableCollection<KeywordData> newList)
			{
				this.Clear();
				foreach (var l in newList) {
					this.Add(l);
				}
			}

			public void Set(List<KeywordData> newList)
			{
				this.Clear();
				foreach (var l in newList) {
					this.Add(l);
				}
			}
		}

		/// <summary>
		/// stores a list of the current keywords being looked for
		/// </summary>
		private KeywordListClass keywordList = new KeywordListClass();
		public KeywordListClass KeywordList
		{
			get
			{
				return keywordList;
			}
			// should not set {} an observable collection because it breaks the binding.
			// instead should only mess around with private value. unoptimally: clear list and add each element.
		}

		private List<TweetData> tweetsProcessed;

		/// <summary>
		/// how many times did we expand the query? 
		/// </summary>
		private ExpansionData currentExpansion;

		// how many times to expand query in each cycle?
		private int maxExpansionCount = 3;

		/// <summary>
		/// how many times did the gatherer finish a sequence of _expansion and restarted for a new set of dates?
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
		// event arguments: sender, sinceDate, untilDate
		public event Action<object, DateTime, DateTime> CycleFinished;
		public event Action<string> Message;
		public event EventHandler Stopped;

		public RestGatherer()
		{
			Reset();

		}

		/// <summary>
		/// clear all.
		/// </summary>
		/// <returns>true if successful, false if cannot continue.</returns>
		public bool Reset()
		{
			keywordList = new KeywordListClass();
			currentExpansion = new ExpansionData(0);
			gatheringCycle = 0;
			// ratelimit counter depends on twitter, so reset will be basically polling twitter for the data.
			var rl = Rest.GetRateLimits_Search();
			if (rl != null) {
				rateLimitCounter = rl.Remaining;
				rateLimitReset = rl.ResetDateTime;
				sinceDate = DateTime.Now;
				untilDate = DateTime.Now;
				tweetsProcessed = new List<TweetData>();
			} else {
				return false;
			}
			return true;
		}

		/// <summary>
		/// should be started in new thread.
		/// </summary>
		public void Algorithm(int expansions, DateTime startDate, DateTime endDate, params string[] filters)
		{
			// set up initial data

			// start date, end date (now). = from parameters

			// init. keywords-filters.
			AddKeywords(0, filters);

			// set up events
			TweetFound += (s, t) => {
				ProcessTweet(t);
				if (currentExpansion.tweets != null) {
					currentExpansion.tweets.Add(t);
				}

			};

			// start algorithm.
			Task.Factory.StartNew(() => {
				while (!stop)
					GatheringCycle2(startDate, endDate);
				Stopped(this, null);
			});
		}

		/// <summary>
		/// attempt at a better version, allowing for external class to handle the expansion
		/// </summary>
		/// <param name="sinceDate"></param>
		/// <param name="untilDate"></param>
		private void GatheringCycle2(DateTime sinceDate, DateTime untilDate)
		{
			this.sinceDate = sinceDate;
			this.untilDate = untilDate;

			try {
				// keywordList contains H0 (keywords from first expansion) and H1 (keywords found after initial search)
				// we must search using the keywords, but ideally without grabbing the tweets that we already grabbed.
				// we already grabbed ALL tweets with keywords from H0, so we should now only search using keywords from H1,
				// to only grab extra tweets. We can even search using NOT H0, to get tweets containing H1 but not H0.
				// same goes for H2, H3, ... until all expansions are done or until no more tweets can be found.
				// what happens from H0 to H1 is the query expansion algorithm.

				// steps:
				//		get all tweets in a cycle (between 2 dates with other restrictions)
				//			= this function
				//		get all tweets in each expansion (using H[0..n] as queries)
				//			= the next loop
				//		get all tweets in a query (split into multiple shorter queries because too many keywords)
				//			= the foreach queries loop
				// function to wait for RestLimits at any step of the way
				//			= inside the smallest loop (which will happen regardless of amount of expansions or query splits

				// for each _expansion
				for (var i = 0; i < maxExpansionCount; i++) {

					currentExpansion = new ExpansionData(i);
					// only perform search if keywords were found
					if (AnyKeywordsInCurrentExpansion()) {
						// split query into multiple shorter ones, within Twitter limits.
						var queries = SplitIntoSmallQueries(keywordList);
						foreach (var q in queries) {
							var sp = ResetSearchParameters(q);
							if (Message != null) Message(sp.SearchQuery);

							// main results loop, waiting for rest limits, etc.
							var results = new List<ITweet>();

							// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
							do {
								if (rateLimitCounter > 0 && !stop) {

									results = Rest.SearchTweets(sp);
									if (results == null) {
										break;
									}
									if (results.Count == 0) {
										break;
									}

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

									// subtract 1 so that we cannot get the same tweets again
									sp.MaxId = long.Parse(minId) - 1;

								} else if (rateLimitCounter <= 0) {
									// check rate limits in case we are wrong and they are not actually zero
									rateLimitCounter = Rest.GetRateLimits_Search().Remaining;
								}
							} while ((results.Count > 0 || rateLimitCounter <= 0) && !stop);

						}

					}
				}

			}
			catch (Exception e) {
				Log.Output("Error in RestGathering algorithm");
				Log.Output(e.ToString());
			}
			// all tweets were gathered in the tweetsToProcess list, which is processed separately by another process

			// the query should be expanded by the other process, and the gathering cycle restarted with settings:
			//			RestartGatheringCycle(untilDate, DateTime.Now);
			CycleFinished(this, sinceDate, untilDate);

		}

		public int maxTweetsPerQuery = 10;

		List<KeywordListClass> SplitIntoSmallQueries(KeywordListClass fullList)
		{
			var klc = new List<KeywordListClass>();

			// twitter documentation shows the amount of tweets allowed (recommended) in each query
			// https://dev.twitter.com/rest/public/search
			// they say limit the search and operators to 10 per search.
			// there can be an error "{error: search too complex}" - that's probably what was happening.
			int smallCount = 0;
			for (int i = 0; i < fullList.Count; i++) {
				if (smallCount >= maxTweetsPerQuery) {
					smallCount = 0;
				}

				if (smallCount == 0) {
					klc.Add(new KeywordListClass());
				}

				klc.Last().Add(fullList[i]);

				smallCount++;
			}

			return klc;
		}

		/// <summary>
		/// perform one gathering cycle until all tweets and expanded versions were gathered, and call new gathering cycle from untilDate until present.
		/// </summary>
		private void GatheringCycle(DateTime sinceDate, DateTime untilDate)
		{
			this.sinceDate = sinceDate;
			this.untilDate = untilDate;

			try {
				// for each _expansion
				for (var i = 0; i < maxExpansionCount; i++) {
					// ########## EXPANSION LOOP ################

					currentExpansion = new ExpansionData(i);

					var sp = ResetSearchParameters();
					// only perform search if keywords were found
					if (AnyKeywordsInCurrentExpansion()) {
						if (Message != null) Message(sp.SearchQuery);

						var results = new List<ITweet>();

						// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
						do {
							if (rateLimitCounter > 0 && !stop) {

								results = Rest.SearchTweets(sp);
								if (results == null) {
									break;
								}
								if (results.Count == 0) {
									break;
								}


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

								// subtract 1 so that we cannot get the same tweets again
								sp.MaxId = long.Parse(minId) - 1;

							} else if (rateLimitCounter <= 0) {
								// check rate limits in case we are wrong and they are not actually zero
								rateLimitCounter = Rest.GetRateLimits_Search().Remaining;
							}
						} while ((results.Count > 0 || rateLimitCounter <= 0) && !stop);

					}

					// ############ _expansion finished ############ 
					ExpansionFinished(this, currentExpansion);
				}
			}
			catch (Exception e) {
				Log.Output("Error in RestGathering algorithm");
				Log.Output(e.ToString());
			}
			// all tweets were gathered in the tweetsToProcess list, which is processed separately by another process

			// the query should be expanded by the other process, and the gathering cycle restarted with settings:
			//			RestartGatheringCycle(untilDate, DateTime.Now);
			CycleFinished(this, sinceDate, untilDate);
		}

		// expands keywords, saves tweets. runs in BG, processes tweets when it has enough resources.
		private void ProcessTweet(TweetData t)
		{
			// only process tweet if it has not been found before.
			if (tweetsProcessed.Any(pt => pt.tweet.Id == t.tweet.Id)) {
				tweetsProcessed.First(pt => pt.tweet.Id == t.tweet.Id).howManyTimesFound++;
				return;
			}

			// how to figure out which expansion the hashtags in this tweet are?
			// find the earliest expansion hashtag inside, and add 1
			var tags = t.GetHashtags();

			// no hashtags? return.
			if (tags.Count == 0) return;

			// choose the tags that exist in the keywordList
			var prevTags = tags.Where(s => HashtagExists(s));
			// if no tags exist in keywordList = impossible, they must exist, because we searched for them ffs.
			//if (prevTags.Count() == 0) return;

			var prevTagsKeywordData = prevTags.Select(k => ExistingKeyword(k));

			if (prevTagsKeywordData.Count() == 0) {
				//AddKeywords(1, tags.ToArray());
				//tweetsProcessed.Add(t);

				return;
			}

			//// find the earliest expansion among the prev ones
			var earliestExp = 0;
			if (prevTagsKeywordData.Count() > 1) {
				earliestExp = prevTagsKeywordData.Aggregate((kd1, kd2) => kd1.Expansion < kd2.Expansion ? kd1 : kd2).Expansion;
			} else {
				earliestExp = prevTagsKeywordData.First().Expansion;
			}

			AddKeywords(earliestExp + 1, tags.ToArray());
			tweetsProcessed.Add(t);

		}

		// true if h exists in the keywordList
		// PLEASE OPTIMIZE ME
		private bool HashtagExists(string h)
		{
			return keywordList.Any(kData => {
				var listTag = kData.Keyword;
				if (listTag.Contains("#")) {
					// remove hashtags
					listTag = listTag.Replace("#", "");
				}
				if (h.Contains("#")) {
					h = h.Replace("#", "");
				}
				return listTag == h;
			});
		}

		private KeywordData ExistingKeyword(string keyword)
		{
			if (HashtagExists(keyword)) {
				return keywordList.First(kData => {
					var listTag = kData.Keyword;
					if (listTag.Contains("#")) {
						listTag = listTag.Replace("#", "");
					}
					if (keyword.Contains("#"))
						keyword = keyword.Replace("#", "");
					return listTag == keyword;
				});
			} else {
				return null;
			}
		}


		/// <summary>
		/// adds list of keywords at current generation.
		/// </summary>
		/// <param name="keywords"></param>
		private void AddKeywords(int expansion, params string[] keywords)
		{
			foreach (var k in keywords) {
				// make them hashtags not anything else.
				var addedK = k;
				if (k.IndexOf("#") != 0) {
					addedK = k.Insert(0, "#");
				}

				// make hashtags lowercase.
				addedK = addedK.ToLower();

				App.Current.Dispatcher.Invoke((Action)(() => {
					if (!keywordList.Any(kkk => kkk.Keyword == addedK)) {
						keywordList.Add(new KeywordData(addedK, expansion));
					} else {
						keywordList.First(kkk => kkk.Keyword == addedK).Count++;
					}
				}));
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
		/// true if there are any new keywords in the current _expansion
		/// </summary>
		/// <returns></returns>
		private bool AnyKeywordsInCurrentExpansion()
		{
			return keywordList.Any(k => k.Expansion == currentExpansion.expansion);
		}

		/// <summary>
		/// sets search parameters based on a local list of keywords
		/// list of query operators from twitter dev site:
		/// https://dev.twitter.com/rest/public/search
		/// also, list of requirements and limitations of search API can be found:
		/// https://dev.twitter.com/rest/public/search
		/// </summary>
		private ITweetSearchParameters ResetSearchParameters(KeywordListClass keywordList)
		{
			string keywordsQuery = "";
			// get all tweets using _keyword list
			bool atLeastOne = false;
			foreach (var k in keywordList) {
				if (k.Expansion == currentExpansion.expansion) {
					if (atLeastOne)
						keywordsQuery += " OR ";
					keywordsQuery += k.Keyword;
					atLeastOne = true;
				}
			}

			// set the latest keywords as search query
			var sp = Rest.GenerateSearchParameters(keywordsQuery);

			sp.SearchType = SearchResultType.Recent;
			sp.TweetSearchFilter = TweetSearchFilter.All;

			sp.Since = sinceDate;

			sp.Until = untilDate;

			sp.MaximumNumberOfResults = 100;

			sp.SinceId = long.MinValue;

			sp.MaxId = long.MaxValue;

			return sp;

		}

		/// <summary>
		/// returns search parameters based on keywords in current _expansion,
		/// and since/until dates. also maxId because we try to get all tweets and no repeats	
		/// </summary>
		private ITweetSearchParameters ResetSearchParameters()
		{
			string keywordsQuery = "";
			// get all tweets using _keyword list
			bool atLeastOne = false;
			foreach (var k in keywordList) {
				if (k.Expansion == currentExpansion.expansion) {
					if (atLeastOne)
						keywordsQuery += " OR ";
					keywordsQuery += k.Keyword;
					atLeastOne = true;
				}
			}

			// set the latest keywords as search query
			var sp = Rest.GenerateSearchParameters(keywordsQuery);

			sp.SearchType = SearchResultType.Recent;
			sp.TweetSearchFilter = TweetSearchFilter.All;

			sp.Since = sinceDate;

			sp.Until = untilDate;

			sp.MaximumNumberOfResults = 100;

			sp.SinceId = long.MinValue;

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

			public List<TweetData> tweets = new List<TweetData>();

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

			// which _expansion cycle first found this tweet?
			public int firstExpansion;

			// how many times was this tweet found after being found once and processed?
			public int howManyTimesFound = 1;

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

		// keywords are added by expanding the original keywords, and each _expansion is expanding the next _expansion etc. until maxExpansionCount
		public class KeywordData
		{
			/// <summary>
			/// the actual hashtag/_keyword string.
			/// </summary>
			private string _keyword;

			public string Keyword
			{
				get { return _keyword; }
				set { _keyword = value; }
			}

			/// <summary>
			/// how many times since the original tweet were tweets with similar keywords pulled?
			/// </summary>
			private int _expansion;

			public int Expansion
			{
				get { return _expansion; }
				set { _expansion = value; }
			}

			private int _count = 0;

			public int Count
			{
				get { return _count; }
				set { _count = value; }
			}

			public KeywordData(string keyword, int generation)
			{
				this._keyword = keyword;
				this._expansion = generation;

			}

		}

	}

}