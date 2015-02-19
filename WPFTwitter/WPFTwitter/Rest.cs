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
			return "yes";
			//Tweetinvi.Streams.Helpers.StreamResultGenerator
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
