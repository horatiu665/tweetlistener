using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace WPFTwitter
{
	public static class Rest
	{

		public static string GetRateLimitsString()
		{
			var r = GetRateLimits_Tweets();
			
			string s = "nig";
			return s;
			//Tweetinvi.Streams.Helpers.StreamResultGenerator
		}

		private static string GetRateLimitString(Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimit rateLimit)
		{
			string s = "";
			s += "\n" + rateLimit.ToString();
			s += "\n" + rateLimit.Limit;
			s += "\n" + rateLimit.Remaining;
			s += "\n" + rateLimit.ResetDateTime;
			return s;
		}

		public static Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimits GetRateLimits_Tweets()
		{
			// only perform search if limit is not reached
			Tweetinvi.Core.Interfaces.Credentials.ITokenRateLimits rateLimits = 
				RateLimit.GetCredentialsRateLimits(Tweetinvi.TwitterCredentials.ApplicationCredentials);

			return rateLimits;

		}

		public static void SearchTweets(string filter)
		{
			//if (GetRateLimits_Tweets().Remaining > 1) {
			//	Console.WriteLine("search");
			//}
		}


	}
}
