using System;
//using System.Collections;
using System.Collections.Generic;
using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces.Models.Parameters;

namespace WPFTwitter
{
	public class Rest
	{
		DatabaseSaver databaseSaver;
		Credentials credentials;
		Log log;
		TweetDatabase tweetDatabase;

		public event Action<ITweet> TweetFound;

		/// <summary>
		/// TODO: reset this counter at the right times.
		/// can query until this is <= 0
		/// </summary>
		public int rateLimitCounter = 0;

		/// <summary>
		/// datetime when rate limit will reset to max value
		/// </summary>
		public DateTime rateLimitReset;

		public bool WaitingForRateLimits { get; set; }

		public int maxTweetsPerQuery = 10;

		public Rest(Credentials creds, DatabaseSaver dbs, Log log, TweetDatabase tdb)
		{
			credentials = creds;
			databaseSaver = dbs;
			this.log = log;
			tweetDatabase = tdb;

			TweetFound += OnTweetFound;
			
		}

		public void TweetsGatheringCycle(DateTime sinceDate, DateTime untilDate, List<string> keywordList)
		{
			// TODO: implement stopping, so if stream is stopped intentionally, this also stops.
			var stop = false;

			log.Output("Start of Rest.TweetsGatheringCycle()");

			try {
				// split query into multiple shorter ones, within Twitter limits.
				var queries = SplitIntoSmallQueries(keywordList);
				foreach (var q in queries) {
					var queryString = "";
					foreach (var s in q) {
						queryString += s + ",";
					}

					var sp = GenerateSearchParameters(queryString);
					sp.SearchType = SearchResultType.Recent;
					//sp.Filters = TweetSearchFilters. .OriginalTweetsOnly;
					sp.Since = sinceDate;
					sp.Until = untilDate;
					sp.MaximumNumberOfResults = 100;
					sp.SinceId = long.MinValue;
					sp.MaxId = long.MaxValue;

					// only search if there are keywords in the query
					if (sp.SearchQuery != "" && !stop) {

						log.Output("Searching for " + sp.SearchQuery);

						// main results loop, waiting for rest limits, etc.
						var results = new List<ITweet>();

						// gathers tweets only when allowed, until there are no more tweets to gather for this set of keywords.
						do {
							if (rateLimitCounter > 0 && !stop) {

								results = SearchTweets(sp);
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

									// tweet found event call (ancient remains from RestGatherer)
									//TweetFound(this, new TweetData(r, gatheringCycle, currentExpansion.expansion));
									if (TweetFound != null) {
										TweetFound(r);
									}
								}

								// subtract 1 so that we cannot get the same tweets again
								sp.MaxId = long.Parse(minId) - 1;

							} else if (rateLimitCounter <= 0) {
								// check rate limits in case we are wrong and they are not actually zero
								var rateLimitsObj = GetRateLimits_Search();
								// wait with checking until rateLimitsResetDate happens
								// TODO

								rateLimitCounter = (rateLimitsObj == null) ? 0 : rateLimitsObj.Remaining;
							}

							// keep doing this while:
							//		there are still results left
							//		or we cannot get tweets due to rate limits
							//		or we cannot get tweets due to errors (in which case we should try again)

						} while ((results.Count > 0 || rateLimitCounter <= 0) && !stop);
					}

				}
			}
			catch (Exception e) {
				log.Output("Error in TweetsGatherCycle() algorithm in Rest.cs");
				log.Output(e.ToString());
			}
			log.Output("End of Rest.TweetsGatheringCycle()");

		}

		void OnTweetFound(ITweet tweet)
		{
			log.Output("Rest gather cycle found the tweet with id " + tweet.IdStr + ": " + tweet.Text);
			databaseSaver.SaveTweet(tweet);
		}

		/// <summary>
		/// compares two ints made into strings because they are too long (tweetId-s)
		/// </summary>
		/// <returns>true when int1 < int2</returns>
		public static bool stringIntSmallerThan(string int1, string int2)
		{
			if (int1.Length > int2.Length) {
				return false;
			} else if (int2.Length < int1.Length) {
				return true;
			} else {
				return (string.Compare(int1, int2) == -1);
			}
		}

		List<List<string>> SplitIntoSmallQueries(List<string> fullList)
		{
			var klc = new List<List<string>>();

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
					klc.Add(new List<string>());
				}

				klc.Last().Add(fullList[i]);

				smallCount++;
			}

			return klc;
		}

		#region viewmodel/controller sort-of

		#region search
		public List<ITweet> SearchTweets(string filter)
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				var tweets = Search_SimpleTweetSearch(filter);
				return tweets;
			} else {
				return null;
			}
		}

		public List<ITweet> SearchTweets(ITweetSearchParameters searchParameters)
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				if (false) {
					#region shit
					// if any param is not properly defined, define it here to default
					if (searchParameters == null) {
						searchParameters = Search.CreateTweetSearchParameter("");
					}

					if (false) {
						if (searchParameters.Until.Year < 2014) {
							searchParameters.Until = DateTime.Now;
						}
					}

					searchParameters.MaximumNumberOfResults = 100;

					if (searchParameters.MaxId == -1) {
						searchParameters.MaxId = long.MaxValue;
					}


					try {
						// print search parameters here with all they contain...?
						string s = "";
						s = ""												// default values:
						+ "\n" + searchParameters.GeoCode					// ???
						+ "\n" + searchParameters.GeoCode.Coordinates.Latitude					// ???
						+ "\n" + searchParameters.GeoCode.Coordinates.Longitude					// ???
						+ "\n" + searchParameters.GeoCode.DistanceMeasure					// ???
						+ "\n" + searchParameters.GeoCode.Radius					// ???
						+ "\n" + searchParameters.Lang						// Undefined
						+ "\n" + searchParameters.Locale					//  
						+ "\n" + searchParameters.MaxId						// -1
						+ "\n" + searchParameters.MaximumNumberOfResults	// 100
						+ "\n" + searchParameters.SearchQuery				// callofduty hehehhe
						+ "\n" + searchParameters.SearchType				// Popular
						+ "\n" + searchParameters.Since						// 01-Jan-01 00:00:00
						+ "\n" + searchParameters.SinceId					// -1
							//+ "\n" + searchParameters.TweetSearchFilter			// All
						+ "\n" + searchParameters.Until;					// 01-Jan-01 00:00:00		
						//Log.Output("Search parameters:" + s);
					}
					catch (NullReferenceException nref) {
						log.Output("One of the search parameters was null. we will find out which one here:");
						log.Output("Could it be this one? " + searchParameters.Lang);// Undefined
						log.Output("Could it be this one? " + searchParameters.Locale);//  
						log.Output("Could it be this one? " + searchParameters.MaxId);// -1
						log.Output("Could it be this one? " + searchParameters.MaximumNumberOfResults);// 100
						log.Output("Could it be this one? " + searchParameters.SearchQuery);// callofduty hehehhe
						log.Output("Could it be this one? " + searchParameters.SearchType);// Popular
						log.Output("Could it be this one? " + searchParameters.Since);// 01-Jan-01 00:00:00
						log.Output("Could it be this one? " + searchParameters.SinceId);// -1
						//log.Output("Could it be this one? " + searchParameters.TweetSearchFilter);// All
						log.Output("Could it be this one? " + searchParameters.Until);// 01-Jan-01 00:00:00		

					}
					#endregion
				}

				try {
					var tweets = Search_SearchTweets(searchParameters);
					return tweets;
				}
				catch (NullReferenceException nullref) {
					log.Output("Null reference at searchTweets function. Cannot fix, will have to ignore.");
					log.Output("Here is the error: " + nullref.ToString());
					// attempting to get error from twitter message
					var ex = Tweetinvi.ExceptionHandler.GetLastException();
					log.Output("Latest exception from Tweetinvi! Status code: " + ex.StatusCode
						+ "\nException description: " + ex.TwitterDescription);
					return null;
				}
			}
			return null;
		}

		// list of query operators from twitter dev site:
		// https://dev.twitter.com/rest/public/search
		public ITweetSearchParameters GenerateSearchParameters(string filter)
		{
			return Search.CreateTweetSearchParameter(filter);

		}
		#endregion

		#region rate limits

		/// <summary>
		/// returns complete rate limits for application, or null when credentials are not set.
		/// </summary>
		/// <returns></returns>
		public Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimits GetRateLimits_All()
		{
			// perform if application is authenticated
			if (Tweetinvi.TwitterCredentials.ApplicationCredentials != null) {
				Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimits rateLimits =
				RateLimit.GetCredentialsRateLimits(Tweetinvi.TwitterCredentials.ApplicationCredentials);

				return rateLimits;
			} else {
				return null;
			}

		}

		/// <summary>
		/// gets rate limit of search API.
		/// </summary>
		/// <returns></returns>
		public Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimit GetRateLimits_Search()
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				var grla = GetRateLimits_All();
				if (grla != null) {
					return grla.ApplicationRateLimitStatusLimit;
				}
			}
			return null;
		}

		#endregion

		#endregion

		// copied and adapted from Examplinvi. acts as model, rest of the Rest class acts as controller, accessed by view (the window/GUI) to do cool stuff.
		#region Tweetinvi functionality

		#region search
		/// <summary>
		/// returns a list of tweets searched for with default parameters and the filter "filter".
		/// </summary>
		/// <param name="filter">filter to use in the search</param>
		/// <returns>list of tweets</returns>
		private List<ITweet> Search_SimpleTweetSearch(string filter)
		{
			// IF YOU DO NOT RECEIVE ANY TWEET, CHANGE THE PARAMETERS!
			List<ITweet> tweets = Search.SearchTweets(filter).ToList();

			return tweets;
		}

		/// <summary>
		/// Searches and returns a list of tweets searched for using all the parameters available
		/// </summary>
		private List<ITweet> Search_SearchTweets(ITweetSearchParameters searchParameters)
		{

			//searchParameter.SetGeoCode(Geo.GenerateCoordinates(-122.398720, 37.781157), 1, DistanceMeasure.Miles);
			//searchParameter.Lang = Language.English;
			//searchParameter.SearchType = SearchResultType.Popular;
			//searchParameter.MaximumNumberOfResults = 100;
			//searchParameter.Since = new DateTime(2013, 12, 1);
			//searchParameter.Until = new DateTime(2013, 12, 11);
			//searchParameter.SinceId = 399616835892781056;
			//searchParameter.MaxId = 405001488843284480;

			if (searchParameters == null) {
				log.Output("Search parameters were null at Rest.cs line 211");
				return null;
			}

			var st = Search.SearchTweets(searchParameters);
			//var st = Search_FilteredSearch(searchParameters.SearchQuery);
			if (st != null) {
				List<ITweet> tweets = st.ToList();
				return tweets;
			} else {
				return null;
			}
		}

		// complicated shit
		private void Search_SearchWithMetadata()
		{
			Search.SearchTweetsWithMetadata("hello");
		}

		/// <summary>
		/// example of search with only a few parameters
		/// </summary>
		private IEnumerable<ITweet> Search_FilteredSearch(string filter)
		{
			var searchParameter = Search.CreateTweetSearchParameter(filter);
			//searchParameter.TweetSearchFilter = TweetSearchFilter.OriginalTweetsOnly;

			var tweets = Search.SearchTweets(searchParameter);
			return tweets;
		}

		/// <summary>
		/// example of search where Tweetinvi handles multiple requests and returns X > 100 results
		/// </summary>
		private void Search_SearchAndGetMoreThan100Results()
		{
			var searchParameter = Search.CreateTweetSearchParameter("us");
			searchParameter.MaximumNumberOfResults = 200;

			var tweets = Search.SearchTweets(searchParameter);
		}
		#endregion

		#endregion
	}
}
