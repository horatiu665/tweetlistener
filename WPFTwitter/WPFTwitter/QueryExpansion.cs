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
			set
			{
				log.Output("Naive expansion percentage set to " + value);
				naiveExpansionPercentage = value;
			}
		}

		private float efronMu = 2000;

		public float EfronMu
		{
			get { return efronMu; }
			set { efronMu = value; }
		}

		private int efronN = 1;

		public int EfronN
		{
			get { return efronN; }
			set { efronN = value; }
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
			log.Output("Expansion: NAIVE\n expanding query on " + tweetPopulation.Count + " tweets");

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
			var expandedKeywords = orderedKeywords.Take((int)Math.Ceiling(orderedKeywords.Count() * (naiveExpansionPercentage / 100f)));

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

		public List<KeywordDatabase.KeywordData> ExpandEfron(
			KeywordDatabase.KeywordListClass keywords,
			TweetDatabase.TweetList tweetPopulation)
		{
			log.Output("Expansion: EFRON\n expanding query on " + tweetPopulation.Count + " tweets");

			// algorithm

			// create language model for each keyword in database
			//		create list of probabilities for each keyword k in database
			//		probability that keyword kk will be found in the same tweet as k
			//		probability is calculated by: number of occurences of kk 
			//			div. by total number of words in documents containing k
			foreach (var keyword in keywords) {
				if (keyword.UseKeyword) {
					LanguageModel lm = new LanguageModel(keyword, keywords, tweetPopulation, efronMu, efronN);
					string s = "Language model for " + keyword.Keyword + ":\n";
					foreach (var kvp in lm.probabilities) {
						s += kvp.Value.ToString("F9") + "\t" + kvp.Key.Keyword + "; \n";
					}
					log.Output(s);
				}
			}

			return null;
		}

	}
}
