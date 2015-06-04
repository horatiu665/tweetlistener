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
using System.ComponentModel;

namespace WPFTwitter
{
	/// <summary>
	/// used to gather tweets and expand the query to include all related hashtags/keywords (customizable) 
	/// until a certain generation (number of iterations from original set of keywords)
	/// </summary>
	public class RestGatherer
	{
		Rest rest;
		Log log;
		TweetDatabase tweetDatabase;
		KeywordDatabase keywordDatabase;

		private bool stop = false;
		public void Stop()
		{
			stop = true;
		}

		/// <summary>
		/// how many times did we expand the query? 
		/// </summary>
		private ExpansionData currentExpansion;

		// how many times to expand query in each cycle?
		private int maxExpansionCount = 3;

		public int MaxExpansionCount
		{
			get { return maxExpansionCount; }
			set { maxExpansionCount = value; }
		}

		/// <summary>
		/// how many times did the gatherer finish a sequence of _expansion and restarted for a new set of dates?
		/// </summary>
		private int gatheringCycle;


		/// <summary>
		/// get tweets since this date up until present day. default = present day.
		/// </summary>
		private DateTime sinceDate;

		/// <summary>
		/// get tweets until this date, to not combine/confuse the repeated sessions of gathering.
		/// after all tweets were gathered once until generation X, start gathering again, change sinceDate to untilDate and untilDate to present date.
		/// </summary>
		private DateTime untilDate;

		public event EventHandler<ITweet> TweetFound;
		public event EventHandler<ExpansionData> ExpansionFinished;
		// event arguments: sender, sinceDate, untilDate
		public event Action<object, DateTime, DateTime> CycleFinished;
		public event Action<string> Message;
		public event EventHandler Stopped;

		public RestGatherer(Rest rest, Log log, TweetDatabase tdb, KeywordDatabase kdb)
		{
			this.rest = rest;
			this.log = log;
			this.tweetDatabase = tdb;
			this.keywordDatabase = kdb;

			Reset();

			// set up events

			TweetFound += (s, t) => {
				var td = new TweetDatabase.TweetData(t, TweetDatabase.TweetData.Sources.Rest, gatheringCycle, currentExpansion.expansion);
				RememberTweetAndExpandKeywords(td);
				if (currentExpansion.tweets != null) {
					currentExpansion.tweets.Add(td);
				}

			};

		}

		/// <summary>
		/// clear all.
		/// </summary>
		/// <returns>true if successful, false if cannot continue.</returns>
		public bool Reset()
		{
			if (keywordDatabase.KeywordList == null) {
				keywordDatabase.KeywordList = new KeywordDatabase.KeywordListClass();
			}
			currentExpansion = new ExpansionData(0);
			gatheringCycle = 0;
			sinceDate = DateTime.Now;
			untilDate = DateTime.Now;
			stop = false;
			return true;
		}

		/// <summary>
		/// should be started in new thread.
		/// </summary>
		public void Algorithm(int expansions, DateTime startDate, DateTime endDate)
		{
			// set up initial data

			// start date, end date (now). = from parameters

			// filters/keywords taken directly from keywordDatabase.
			if (keywordDatabase.KeywordList.Count == 0) {
				log.Output("There are no keywords in the list. Cannot expand on an empty stomach. Please add some using the Keywords panel.");
				return;
			}

			// start algorithm.
			Task.Factory.StartNew(() => {
				while (!stop)
					GatheringCycle2(startDate, endDate);
				Stopped(this, null);
			});
		}

		/// <summary>
		/// attempt at a better version
		/// </summary>
		/// <param name="sinceDate"></param>
		/// <param name="untilDate"></param>
		private async void GatheringCycle2(DateTime sinceDate, DateTime untilDate)
		{
			if (rest.WaitingForRateLimits) {
				return;
			}

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
				for (var i = 0; i < MaxExpansionCount; i++) {

					currentExpansion = new ExpansionData(i);
					// only perform search if keywords were found
					if (AnyKeywordsInCurrentExpansion()) {
						// split query into multiple shorter ones, within Twitter limits.
						var queries = SplitIntoSmallQueries(keywordDatabase.KeywordList);
						foreach (var q in queries) {
							var sp = ResetSearchParameters(q, currentExpansion.expansion);

							// only search if there are keywords in the query
							if (sp.SearchQuery != "" && !stop) {

								if (Message != null) Message(sp.SearchQuery);

								// main results loop, waiting for rest limits, etc.
								var results = new List<ITweet>();

								// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
								do {
									if (rest.rateLimitCounter > 0 && !stop) {

										results = rest.SearchTweets(sp);
										if (results == null) {
											break;
										}
										if (results.Count == 0) {
											break;
										}

										rest.rateLimitCounter--;

										// save minId to use for next batch of tweets
										var minId = results[0].IdStr;
										foreach (var r in results) {
											// find minId among results.
											if (Rest.stringIntSmallerThan(r.IdStr, minId)) {
												minId = r.IdStr;
											}

											TweetFound(this, r);

										}

										// subtract 1 so that we cannot get the same tweets again
										sp.MaxId = long.Parse(minId) - 1;

									} else if (rest.rateLimitCounter <= 0) {
										// check rate limits in case we are wrong and they are not actually zero
										//var rateLimitsObj = rest.GetRateLimits_Search();
										rest.WaitingForRateLimits = true;
										var rateLimitsObj = await RateLimitAsync.GetCurrentCredentialsRateLimits();
										rest.WaitingForRateLimits = false;
										rest.rateLimitCounter = (rateLimitsObj == null) ? 0 : rateLimitsObj.ApplicationRateLimitStatusLimit.Remaining;

									}

									// keep doing this while:
									//		there are still results left
									//		or we cannot get tweets due to rate limits
									//		or we cannot get tweets due to errors (in which case we should try again)

								} while ((results.Count > 0 || rest.rateLimitCounter <= 0) && !stop);
							}
						}

					}
				}

			}
			catch (Exception e) {
				log.Output("Error in RestGathering algorithm");
				log.Output(e.ToString());
			}


			// all tweets were gathered in the tweetsToProcess list, which is processed separately by another process

			// the query should be expanded by the other process, and the gathering cycle restarted with settings:
			//			RestartGatheringCycle(untilDate, DateTime.Now);
			CycleFinished(this, sinceDate, untilDate);

		}

		List<KeywordDatabase.KeywordListClass> SplitIntoSmallQueries(KeywordDatabase.KeywordListClass fullList)
		{
			var klc = new List<KeywordDatabase.KeywordListClass>();

			// twitter documentation shows the amount of tweets allowed (recommended) in each query
			// https://dev.twitter.com/rest/public/search
			// they say limit the search and operators to 10 per search.
			// there can be an error "{error: search too complex}" - that's probably what was happening.
			int smallCount = 0;
			for (int i = 0; i < fullList.Count; i++) {
				if (smallCount >= rest.maxTweetsPerQuery) {
					smallCount = 0;
				}

				if (smallCount == 0) {
					klc.Add(new KeywordDatabase.KeywordListClass());
				}

				klc.Last().Add(fullList[i]);

				smallCount++;
			}

			return klc;
		}

		/// <summary>
		/// old version. try GatheringCycle2
		/// perform one gathering cycle until all tweets and expanded versions were gathered, and call new gathering cycle from untilDate until present.
		/// </summary>
		private void GatheringCycle(DateTime sinceDate, DateTime untilDate)
		{
			this.sinceDate = sinceDate;
			this.untilDate = untilDate;

			try {
				// for each _expansion
				for (var i = 0; i < MaxExpansionCount; i++) {
					// ########## EXPANSION LOOP ################

					currentExpansion = new ExpansionData(i);

					var sp = ResetSearchParameters();
					// only perform search if keywords were found
					if (AnyKeywordsInCurrentExpansion()) {
						if (Message != null) Message(sp.SearchQuery);

						var results = new List<ITweet>();

						// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
						do {
							if (rest.rateLimitCounter > 0 && !stop) {

								results = rest.SearchTweets(sp);
								if (results == null) {
									break;
								}
								if (results.Count == 0) {
									break;
								}


								rest.rateLimitCounter--;

								// save minId to use for next batch of tweets
								var minId = results[0].IdStr;
								foreach (var r in results) {
									// find minId among results.
									if (Rest.stringIntSmallerThan(r.IdStr, minId)) {
										minId = r.IdStr;
									}

									TweetFound(this, r);

								}

								// subtract 1 so that we cannot get the same tweets again
								sp.MaxId = long.Parse(minId) - 1;

							} else if (rest.rateLimitCounter <= 0) {
								// check rate limits in case we are wrong and they are not actually zero
								rest.rateLimitCounter = rest.GetRateLimits_Search().Remaining;
							}
						} while ((results.Count > 0 || rest.rateLimitCounter <= 0) && !stop);

					}

					// ############ _expansion finished ############ 
					ExpansionFinished(this, currentExpansion);
				}
			}
			catch (Exception e) {
				log.Output("Error in RestGathering algorithm");
				log.Output(e.ToString());
			}
			// all tweets were gathered in the tweetsToProcess list, which is processed separately by another process

			// the query should be expanded by the other process, and the gathering cycle restarted with settings:
			//			RestartGatheringCycle(untilDate, DateTime.Now);
			CycleFinished(this, sinceDate, untilDate);
		}

		// expands keywords, saves tweets. runs in BG, processes tweets when it has enough resources.
		private void RememberTweetAndExpandKeywords(TweetDatabase.TweetData t)
		{
			// only process tweet if it has not been found before.
			if (tweetDatabase.Tweets.Any(pt => pt.tweet.Id == t.tweet.Id)) {
				tweetDatabase.Tweets.First(pt => pt.tweet.Id == t.tweet.Id).howManyTimesFound++;
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
		}

		// true if h exists in the keywordList
		// PLEASE OPTIMIZE ME
		private bool HashtagExists(string h)
		{
			return keywordDatabase.KeywordList.Any(kData => {
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

		private KeywordDatabase.KeywordData ExistingKeyword(string keyword)
		{
			if (HashtagExists(keyword)) {
				return keywordDatabase.KeywordList.First(kData => {
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
					if (!keywordDatabase.KeywordList.Any(kkk => kkk.Keyword == addedK)) {
						keywordDatabase.KeywordList.Add(new KeywordDatabase.KeywordData(addedK, expansion));
					} else {
						int i = 0;
						for (i = 0; i < keywordDatabase.KeywordList.Count; i++) {
							if (keywordDatabase.KeywordList[i].Keyword == addedK) break;
						}
						// property changed! even if we did not add to the list.
						// add and remove shit to update list.
						var newKey = new KeywordDatabase.KeywordData(keywordDatabase.KeywordList[i].Keyword, keywordDatabase.KeywordList[i].Expansion);
						newKey.Count = keywordDatabase.KeywordList[i].Count + 1;
						keywordDatabase.KeywordList.RemoveAt(i);
						keywordDatabase.KeywordList.Insert(i, newKey);


						// old method, does not update list:
						//keywordDatabase.KeywordList.First(kkk => kkk.Keyword == addedK).Count++;

					}
				}));
			}
		}

		/// <summary>
		/// true if there are any new keywords in the current _expansion
		/// </summary>
		/// <returns></returns>
		private bool AnyKeywordsInCurrentExpansion()
		{
			return keywordDatabase.KeywordList.Any(k => k.Expansion == currentExpansion.expansion);
		}

		private ITweetSearchParameters ResetSearchParameters(KeywordDatabase.KeywordListClass keywordList, int onlyFromExpansion = -1)
		{
			return ResetSearchParameters(keywordList, onlyFromExpansion, null);
		}

		/// <summary>
		/// ONLY RETURNS KEYWORDS FROM CURRENT EXPANSION and given list
		/// sets search parameters based on a local list of keywords.
		/// list of query operators from twitter dev site:
		/// https://dev.twitter.com/rest/public/search
		/// also, list of requirements and limitations of search API can be found:
		/// https://dev.twitter.com/rest/public/search
		/// </summary>
		private ITweetSearchParameters ResetSearchParameters(KeywordDatabase.KeywordListClass keywordList, int onlyFromExpansion, ITweetSearchParameters searchParams)
		{
			string keywordsQuery = "";
			// get all tweets using _keyword list
			bool atLeastOne = false;
			foreach (var k in keywordList) {
				if ((onlyFromExpansion == -1) ^ (k.Expansion == onlyFromExpansion)) {
					if (atLeastOne)
						keywordsQuery += " OR ";
					keywordsQuery += k.Keyword;
					atLeastOne = true;
				}
			}

			// set the latest keywords as search query
			var sp = rest.GenerateSearchParameters(keywordsQuery);

			sp.SearchType = SearchResultType.Recent;
			//sp.Filters = TweetSearchFilters. .OriginalTweetsOnly;

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
			foreach (var k in keywordDatabase.KeywordList) {
				if (k.Expansion == currentExpansion.expansion) {
					if (atLeastOne)
						keywordsQuery += " OR ";
					keywordsQuery += k.Keyword;
					atLeastOne = true;
				}
			}

			// set the latest keywords as search query
			var sp = rest.GenerateSearchParameters(keywordsQuery);

			sp.SearchType = SearchResultType.Recent;
			//sp.TweetSearchFilter = TweetSearchFilter.OriginalTweetsOnly;

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

			public List<TweetDatabase.TweetData> tweets = new List<TweetDatabase.TweetData>();

			public ExpansionData(int num)
			{
				expansion = num;
			}

		}


	}

}