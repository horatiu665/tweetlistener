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
	public static class Rest
	{
		private static List<ITweet> lastTweets = new List<ITweet>();

		/// <summary>
		/// adds previously returned tweets to database, complementing the Stream log with extra results.
		/// </summary>
		public static void AddLastTweetsToDatabase()
		{
			foreach (var t in lastTweets) {
				// use DatabaseSaver to save each tweet just like we do with the streaming
				DatabaseSaver.SendTweetToDatabase(t);
				
			}
		}

		#region viewmodel/controller sort-of

		#region search
		public static List<ITweet> SearchTweets(string filter)
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				var tweets = Search_SimpleTweetSearch(filter);
				lastTweets.Clear();
				foreach (var t in tweets) {
					lastTweets.Add(t);
				}
				return tweets;
			} else {
				return null;
			}
		}

		public static List<ITweet> SearchTweets(ITweetSearchParameters searchParameters)
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				// if any param is not properly defined, define it here to default
				if (searchParameters.GeoCode == null) {
					searchParameters.SetGeoCode(Geo.GenerateCoordinates(0, 0), 100000, DistanceMeasure.Miles);
				}
				if (searchParameters.Lang == null) {
					searchParameters.Lang = Language.English;
				}
				if (searchParameters.Locale == null) {
					searchParameters.Locale = "";
				}
				if (searchParameters.SearchType == null) {
					searchParameters.SearchType = SearchResultType.Recent;
				}
				if (searchParameters.MaximumNumberOfResults == null) 
					searchParameters.MaximumNumberOfResults = 100;
				if (searchParameters.Since == null)
					searchParameters.Since = DateTime.Now.AddDays(-1);
				if (searchParameters.Until == null)
					searchParameters.Until = DateTime.Now;
				if (searchParameters.SinceId == null) {
					searchParameters.SinceId = -1;
				}
				if (searchParameters.MaxId == null) {
					searchParameters.MaxId = long.MaxValue;
				}

				
				var tweets = Search_SearchTweets(searchParameters);
				lastTweets.Clear();
				foreach (var t in tweets) {
					lastTweets.Add(t);
				}
				return tweets;
			}
			return null;
		}

		public static ITweetSearchParameters GenerateSearchParameters(string filter)
		{
			return Search.GenerateTweetSearchParameter(filter);

		}
		#endregion

		#region rate limits

		/// <summary>
		/// returns complete rate limits for application, or null when credentials are not set.
		/// </summary>
		/// <returns></returns>
		public static Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimits GetRateLimits_All()
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
		public static Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimit GetRateLimits_Search()
		{
			if (TwitterCredentials.ApplicationCredentials != null) {
				return GetRateLimits_All().ApplicationRateLimitStatusLimit;
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
		private static List<ITweet> Search_SimpleTweetSearch(string filter)
		{
			// IF YOU DO NOT RECEIVE ANY TWEET, CHANGE THE PARAMETERS!
			List<ITweet> tweets = Search.SearchTweets(filter).ToList();

			return tweets;
		}

		/// <summary>
		/// Searches and returns a list of tweets searched for using all the parameters available
		/// </summary>
		private static List<ITweet> Search_SearchTweets(ITweetSearchParameters searchParameters)
		{

			//searchParameter.SetGeoCode(Geo.GenerateCoordinates(-122.398720, 37.781157), 1, DistanceMeasure.Miles);
			//searchParameter.Lang = Language.English;
			//searchParameter.SearchType = SearchResultType.Popular;
			//searchParameter.MaximumNumberOfResults = 100;
			//searchParameter.Since = new DateTime(2013, 12, 1);
			//searchParameter.Until = new DateTime(2013, 12, 11);
			//searchParameter.SinceId = 399616835892781056;
			//searchParameter.MaxId = 405001488843284480;

			List<ITweet> tweets = Search.SearchTweets(searchParameters).ToList();
			return tweets;
		}

		// complicated shit
		private static void Search_SearchWithMetadata()
		{
			Search.SearchTweetsWithMetadata("hello");
		}

		/// <summary>
		/// example of search with only a few parameters
		/// </summary>
		private static void Search_FilteredSearch()
		{
			var searchParameter = Search.GenerateTweetSearchParameter("#tweetinvi");
			searchParameter.TweetSearchFilter = TweetSearchFilter.OriginalTweetsOnly;

			var tweets = Search.SearchTweets(searchParameter);
			tweets.ForEach(t => Console.WriteLine(t.Text));
		}

		/// <summary>
		/// example of search where Tweetinvi handles multiple requests and returns X > 100 results
		/// </summary>
		private static void Search_SearchAndGetMoreThan100Results()
		{
			var searchParameter = Search.GenerateTweetSearchParameter("us");
			searchParameter.MaximumNumberOfResults = 200;

			var tweets = Search.SearchTweets(searchParameter);
			tweets.ForEach(t => Console.WriteLine(t.Text));
		}
		#endregion

		#endregion
	}
}
