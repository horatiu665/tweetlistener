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
		private DatabaseSaver databaseSaver;
		private Credentials credentials;
		Log log;

		private List<ITweet> lastTweets = new List<ITweet>();

		public Rest(Credentials creds, DatabaseSaver dbs, Log log)
		{
			credentials = creds;
			databaseSaver = dbs;
			this.log = log;
		}

		/// <summary>
		/// adds previously returned tweets to database, complementing the Stream log with extra results.
		/// </summary>
		public void AddLastTweetsToDatabase()
		{
			foreach (var t in lastTweets) {
				// use DatabaseSaver to save each tweet just like we do with the streaming
				databaseSaver.SendTweetToDatabase(t);

			}
		}

		#region viewmodel/controller sort-of

		#region search
		public List<ITweet> SearchTweets(string filter)
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
					lastTweets.Clear();
					foreach (var t in tweets) {
						lastTweets.Add(t);
					}
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
			//tweets.ForEach(t => Console.WriteLine(t.Text));
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
			tweets.ForEach(t => Console.WriteLine(t.Text));
		}
		#endregion

		#endregion
	}
}
