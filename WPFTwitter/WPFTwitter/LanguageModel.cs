using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WPFTwitter
{
	/// <summary>
	/// A language model for a certain word W is a dictionary of keyword/probability pairs,
	/// describing how probable is that each keyword k[i] will be found in the same tweet as W.
	/// </summary>
	public class LanguageModel
	{
		public KeywordDatabase.KeywordData keyword;

		public Dictionary<KeywordDatabase.KeywordData, float> probabilities = new Dictionary<KeywordDatabase.KeywordData, float>();

		/// <summary>
		/// Initializes the probabilities dictionary as a language model of the keyword based on the tweet population and keyword list provided.
		/// </summary>
		/// <param name="keyword">keyword for which we have the language model</param>
		/// <param name="keywordList">other keywords in the vocabulary</param>
		/// <param name="tweetPopulation">document/corpus/all words to base the language model on</param>
		public LanguageModel(KeywordDatabase.KeywordData keyword, KeywordDatabase.KeywordListClass keywordList, TweetDatabase.TweetList tweetPopulation, float smoothingMu, int efronN)
		{
			// compute probability for each k in keywordList to appear in a tweet from tweetPopulation together with keyword
			foreach (var k in keywordList) {

				var thiskey = k.Keyword;
				if (thiskey.Contains("#")) {
					thiskey = thiskey.Substring(1);
				}

				// maxmimum likelihood of K occurring along with keyword in a tweet is:
				// the number of times it appears in the corpus
				// div. by the amount of words in the corpus

				var numAppearancesOfK = 0;
				var numWordsInTweetPopulation = 0;
				for (int i = 0; i < tweetPopulation.Count; i++) {
					// only if it contains the original keyword
					if (tweetPopulation[i].ContainsHashtag(keyword.Keyword)) {
						var tweet = tweetPopulation[i].tweet;

						// count occurrences of thiskey in tweetText
						numAppearancesOfK += tweet.Hashtags.Count(h => h.Text.ToLower() == thiskey);

						// count words in tweetText //////////////////////////////////////////////////////////////////////////////////////////////// pls improve. right now it counts hyperlinks, etc.
						numWordsInTweetPopulation += tweet.Hashtags.Count;
					}
				}

				// max likelihood 
				var maxLikelihoodOfK = numAppearancesOfK / (float)numWordsInTweetPopulation;

				probabilities.Add(k, maxLikelihoodOfK);
			}

			var numWords = 0;
			for (int i = 0; i < tweetPopulation.Count; i++) {
				if (tweetPopulation[i].ContainsHashtag(keyword.Keyword)) {
					numWords += tweetPopulation[i].GetHashtags().Count;
				}
			}

			// after all max likelihood estimators are calculated, smooth model by some smoothing method
			probabilities = SmoothProbabilities(probabilities, efronN, smoothingMu);

		}

		Dictionary<KeywordDatabase.KeywordData, float> SmoothProbabilities(Dictionary<KeywordDatabase.KeywordData, float> probabilities, int N, float smoothingConstantMu = 2000)
		{
			var newProbs = new Dictionary<KeywordDatabase.KeywordData, float>();
			foreach (var xi in probabilities) {
				newProbs[xi.Key] = (xi.Value + smoothingConstantMu) / (N + smoothingConstantMu * probabilities.Count);
				//newProbs[xi.Key] = xi.Value;
			}

			return newProbs;
		}


		/// <summary>
		/// smooths shit
		/// </summary>
		/// <param name="occurrences"></param>
		/// <param name="contextSize"></param>
		/// <param name="_mu"></param>
		/// <param name="_muTimesCollectionFrequency"></param>
		/// <returns></returns>
		double scoreOccurrence(double occurrences, int contextSize, double _mu, double _muTimesCollectionFrequency)
		{
			double seen = ((double)(occurrences) + _muTimesCollectionFrequency) / ((double)(contextSize) + _mu);
			return Math.Log(seen);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="occurrences"></param>
		/// <param name="contextSize"></param>
		/// <param name="documentOccurrences"></param>
		/// <param name="documentLength"></param>
		/// <param name="_mu">default 2500, Efron uses 2000</param>
		/// <param name="_muTimesCollectionFrequency">collectionFrequency = </param>
		/// <param name="_docmu"></param>
		/// <returns></returns>
		double scoreOccurrence(double occurrences, int contextSize, double documentOccurrences, int documentLength, double _mu, double _muTimesCollectionFrequency, double _docmu)
		{
			//two level Dir Smoothing!
			//         tf_E + documentMu*P(t|D)
			// P(t|E)= ------------------------
			//          extentlen + documentMu
			//                  mu*P(t|C) + tf_D
			// where P(t|D)= ---------------------
			//                   doclen + mu
			// if the _docmu parameter is the default, do collection level
			// smoothing only.
			if (_docmu < 0)
				return scoreOccurrence(occurrences, contextSize, _mu, _muTimesCollectionFrequency);
			else {
				double seen = (occurrences + _docmu * (_muTimesCollectionFrequency + documentOccurrences) / ((double)(documentLength) + _mu)) / ((double)(contextSize) + _docmu);
				return Math.Log(seen);
			}
		}

	}
}
