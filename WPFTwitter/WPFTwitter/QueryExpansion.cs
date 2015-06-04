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

		private float naiveExpansionPercentage = 10;

		public float NaiveExpansionPercentage
		{
			get { return naiveExpansionPercentage; }
			set { naiveExpansionPercentage = value; }
		}


		public QueryExpansion(Log log)
		{
			this.log = log;
		}

		public List<KeywordDatabase.KeywordData> Expand(List<KeywordDatabase.KeywordData> keywords, List<TweetDatabase.TweetData> tweetPopulation)
		{
			return ExpandNaive(keywords, tweetPopulation);
		}


		public List<KeywordDatabase.KeywordData> ExpandNaive(List<KeywordDatabase.KeywordData> keywords, List<TweetDatabase.TweetData> tweetPopulation)
		{
			log.Output("Expanding query on " + tweetPopulation.Count + " tweets");

			// find keywords from tweetPopulation (and their count)
			Dictionary<string, int> keywordsInTweets = new Dictionary<string, int>();
			foreach (var t in tweetPopulation) {
				foreach (var ht in t.GetHashtags()) {
					var tag = ht.ToLower();
					if (keywordsInTweets.Keys.Contains(tag)) {
						keywordsInTweets[tag]++;
					} else {
						keywordsInTweets[tag] = 1;
					}
				}
			}

			// remove the ones that are already in the keywords list
			foreach (var k in keywords) {
				var toRemove = k.Keyword;
				toRemove = toRemove.Replace("#", "");
				keywordsInTweets.Remove(toRemove);
			}

			// order by count somehow
			var orderedKeywords = keywordsInTweets.OrderByDescending(kvp => kvp.Value);

			// return original query + X% top most frequent new keywords
			var expandedKeywords = orderedKeywords.Take((int)Math.Ceiling(orderedKeywords.Count() * (naiveExpansionPercentage/100f)));

			expandedKeywords = expandedKeywords.Where(kvp => kvp.Key.Any(cha => cha != ' '));

			var s = "";
			expandedKeywords.ToList().ForEach(kvp => s += kvp.Key + ", ");

			if (expandedKeywords.Count() == 0) {
				log.Output("No new keywords were found");
			} else {
				log.Output("New keywords after expansion: " + s);
			}

			// create new keywordData for the new keywords found
			var newKeywordList = new List<KeywordDatabase.KeywordData>();
			foreach (var e in expandedKeywords) {
				newKeywordList.Add(new KeywordDatabase.KeywordData("#" + e.Key, 0));
			}

			return keywords.Concat(newKeywordList).ToList();
		}

	}
}
