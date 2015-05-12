using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace WPFTwitter
{
	public class QueryExpansion
	{

		Log log;

		public QueryExpansion(Log log)
		{
			this.log = log;
		}

		public List<string> Expand(List<string> query, List<ITweet> tweetPopulation)
		{
			return ExpandNaive(query, tweetPopulation);
		}


		private List<string> ExpandNaive(List<string> query, List<ITweet> tweetPopulation)
		{
			log.Output("Expanding query on " + tweetPopulation.Count + " tweets");

			// find keywords from tweetPopulation (and their count)
			Dictionary<string, int> keywordsInTweets = new Dictionary<string, int>();
			foreach (var t in tweetPopulation) {
				foreach (var ht in t.Hashtags) {
					var tag = ht.Text.ToLower();
					if (keywordsInTweets.Keys.Contains(tag)) {
						keywordsInTweets[tag]++;
					} else {
						keywordsInTweets[tag] = 1;
					}
				}
			}

			// order by count somehow
			var orderedKeywords = keywordsInTweets.OrderByDescending(kvp => kvp.Value);

			var expandedKeywords = orderedKeywords.Take((int)(orderedKeywords.Count() * 0.1f));

			var s = "";
			expandedKeywords.ToList().ForEach(kvp => s += kvp.Key + ", ");

			log.Output("New keywords after expansion: " + s);

			// return original query + X% top most frequent new keywords

			return query.Concat(expandedKeywords.Select(kvp => kvp.Key)).ToList();
		}

	}
}
