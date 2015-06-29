using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WPFTwitter
{

	/// <summary>
	/// A language model for a certain word W is a dictionary of word_i/probability pairs,
	/// describing how probable is that each word_i k[i] will be found in the same tweet as W.
	/// </summary>
	public class LanguageModel
	{
		public KeywordDatabase.KeywordData keyword;

		public Dictionary<KeywordDatabase.KeywordData, float> probabilities = new Dictionary<KeywordDatabase.KeywordData, float>();

		/// <summary>
		/// previously calculated KL divergence, for use in the algorithm (so we don't calculated twice in a row)
		/// </summary>
		public double KldResult = 0;

		public enum SmoothingMethods
		{
			MLE, // maximum likelihood estimator
			BayesianDirichlet, // bayesian smoothing with dirichlet priors
		}

		/// <summary>
		/// Initializes the probs dictionary as a language model of the word_i based on the tweet population and word_i list provided.
		/// </summary>
		/// <param name="word_i">word_i for which we have the language model</param>
		/// <param name="vocabulary">other keywords in the vocabulary</param>
		/// <param name="tweetCollection">document/corpus/all words to base the language model on</param>
		public LanguageModel(KeywordDatabase.KeywordData word_i, KeywordDatabase.KeywordListClass vocabulary, List<TweetDatabase.TweetData> tweetCollection, Dictionary<string, int> keywordCountsC, SmoothingMethods smoothingMethod, float smoothingMu = 2000f)
		{
			keyword = word_i;

			// use maximum likelihood estimator?
			if (smoothingMethod == SmoothingMethods.MLE) {
				//MLE(word_i.Keyword, vocabulary, tweetCollection, ref probabilities);
			} else {
				BayesianSmoothing(word_i.Keyword, vocabulary, tweetCollection, ref probabilities, keywordCountsC, smoothingMu);
			}

			// after all max likelihood estimators are calculated, smooth model by some smoothing method
			//probabilities = BayesianSmoothing(probabilities, smoothingMu);

			//var numWords = 0;
			//for (int i = 0; i < tweetCollection.Count; i++) {
			//	if (tweetCollection[i].ContainsHashtag(word_i.Keyword)) {
			//		numWords += tweetCollection[i].GetHashtags().Count;
			//	}
			//}

		}

		/// <summary>
		/// Bayesian smoothing with Dirichlet priors, based on Efron and other sources. Details in paper.
		/// </summary>
		/// <param name="probs"></param>
		/// <param name="N"></param>
		/// <param name="smoothingConstantMu"></param>
		/// <returns></returns>
		void BayesianSmoothing(string keyword, KeywordDatabase.KeywordListClass vocabulary, List<TweetDatabase.TweetData> tweetCollection,
			ref Dictionary<KeywordDatabase.KeywordData, float> probabilities, Dictionary<string, int> keywordCountsC, float smoothingConstantMu)
		{
			probabilities.Clear();

			// D = docs containing word_i
			var tweetsContainingHashtag = tweetCollection.Where(td => td.ContainsHashtag(keyword)).ToList();

			// num words in D
			var numWordsInSample = 0;
			Dictionary<string, int> keywordCountsD = new Dictionary<string, int>();
			foreach (var t in tweetsContainingHashtag) {
				foreach (var ht in t.GetHashtags()) {
					var tag = ht.ToLower();
					if (keywordCountsD.Keys.Contains(tag)) {
						keywordCountsD[tag]++;
					} else {
						keywordCountsD[tag] = 1;
					}
					numWordsInSample++;
				}
			}

			var numWordsInCollection = 0;
			if (keywordCountsC == null) {
				keywordCountsC = new Dictionary<string, int>();
				foreach (var t in tweetCollection) {
					foreach (var ht in t.GetHashtags()) {
						var tag = ht.ToLower();
						if (keywordCountsC.Keys.Contains(tag)) {
							keywordCountsC[tag]++;
						} else {
							keywordCountsC[tag] = 1;
						}
						numWordsInCollection++;
					}
				}
			} else {
				numWordsInCollection = keywordCountsC.Sum(kvp => kvp.Value);
			}
			// now we have the counts of keywords.


			float optimize1 = smoothingConstantMu / (float)numWordsInCollection;
			float optimize2 = numWordsInSample + smoothingConstantMu;

			// for each, use formula in paper, or see reference:
			// C. Zhai and J. Lafferty, "A Study of Smoothing Methods for Language Models Applied to Information Retrieval," ACM Transactions on Information Systems, vol. 22, no. 2, pp. 179-214, 2004.
			foreach (var k in vocabulary) {

				var word_k = k.Keyword;
				// guarranteed that the keywordList will only contain hashtags with the # in front.
				//if (word_k.Contains("#")) {
				word_k = word_k.Substring(1);
				//}

				// num appearances of k in D
				var countWordKInD = keywordCountsD.ContainsKey(word_k) ? keywordCountsD[word_k] : 0;



				// num appearances of k in C
				var countWordKInC = keywordCountsC.ContainsKey(word_k) ? keywordCountsC[word_k] : 0;
				
				// THIS IS WRONG. IT COUNTS THE TWEETS IN WHICH IT APPEARS, NOT THE AMOUNT OF TIMES IT APPEARS IN THE CORPUS.
				//var countWordKInC = k.Count;

				// smooth prob
				//var smoothProb = (countWordKInD + smoothingConstantMu * countWordKInC / numWordsInCollection) / ((float)numWordsInSample + smoothingConstantMu);
				var smoothProb = (countWordKInD + countWordKInC * optimize1) / optimize2;

				probabilities.Add(k, smoothProb);
			}

		}

		void Dirichlet()
		{

		}


		/// <summary>
		/// Based on Wikipedia formula https://en.wikipedia.org/wiki/Kullback%E2%80%93Leibler_divergence
		/// The divergence can be considered the "distance" between two probability distributions.
		/// Equal to zero when the two distributions are equal.
		/// </summary>
		/// <param name="probability1"></param>
		/// <param name="probability2"></param>
		/// <returns></returns>
		public static double KlDivergence(List<float> probability1, List<float> probability2)
		{
			double sum = 0f;
			for (int i = 0; i < probability1.Count; i++) {
				if (probability1[i] != 0 && probability2[i] != 0) {
					sum += ((double)probability1[i] * Math.Log((double)probability1[i] / (double)probability2[i]));
				}
			}
			return sum;
		}

		public string ToString()
		{
			string s = "Language model for: " + keyword.Keyword + "\n";
			foreach (var p in probabilities) {
				s += p.Value.ToString("F8") + " " + p.Key.Keyword + "\n";
			}
			return s;
		}

	}

	public class QueryModel
	{
		public string query;

		public Dictionary<KeywordDatabase.KeywordData, float> probabilities = new Dictionary<KeywordDatabase.KeywordData, float>();

		public QueryModel(string query, KeywordDatabase.KeywordListClass vocabulary, List<TweetDatabase.TweetData> tweetCollection, out List<TweetDatabase.TweetData> tweetsReturnedByModel)
		{
			this.query = query;
			MLE(query, vocabulary, tweetCollection, ref probabilities, out tweetsReturnedByModel);

		}

		/// <summary>
		/// See MLE function in LanguageModel for more details
		/// </summary>
		void MLE(string query, KeywordDatabase.KeywordListClass vocabulary, List<TweetDatabase.TweetData> tweetCollection, ref Dictionary<KeywordDatabase.KeywordData, float> probs, 
			out List<TweetDatabase.TweetData> tweetsReturnedByQuery)
		{
			probs.Clear();

			var queryParts = query.Split(',').Where(qp => qp.Replace(" ", "") != "").ToList();

			// D = docs containing word_i
			tweetsReturnedByQuery = tweetCollection.Where(td => queryParts.Any(qp => td.ContainsHashtag(qp))).ToList();

			// num words in D
			var numWordsInSample = 0;
			for (int i = 0; i < tweetsReturnedByQuery.Count; i++) {
				numWordsInSample += tweetsReturnedByQuery[i].tweet.Hashtags.Count;
			}

			// compute probability for each k in vocabulary to appear in a tweet from tweetCollection together with word_i
			
			// count all keyword counts by going once through tweets
			Dictionary<string, int> keywordCounts = new Dictionary<string, int>();
			foreach (var t in tweetsReturnedByQuery) {
				foreach (var ht in t.GetHashtags()) {
					var tag = ht.ToLower();
					if (keywordCounts.Keys.Contains(tag)) {
						keywordCounts[tag]++;
					} else {
						keywordCounts[tag] = 1;
					}
				}
			}
			// now we have the counts of keywords.

			foreach (var k in vocabulary) {

				var word_k = k.Keyword;
				//if (word_k.Contains("#")) {
				word_k = word_k.Substring(1);
				//}

				// maxmimum likelihood of K occurring along with word_i in a tweet is:
				// the number of times it appears in D
				// div. by the amount of words in D

				// num appearances of k in D
				var numAppearancesOfK = keywordCounts.ContainsKey(word_k) ? keywordCounts[word_k] : 0;

				// max likelihood 
				var maxLikelihoodOfK = numAppearancesOfK / (float)numWordsInSample;

				probs.Add(k, maxLikelihoodOfK);
			}

			// MLE calculated in the probs[] array.

		}

		public void ApplyFeedback(Dictionary<KeywordDatabase.KeywordData, float> newProbabilities, float weight)
		{
			foreach (var k in newProbabilities.Keys) {
				probabilities[k] = probabilities[k] * (1 - weight) + weight * newProbabilities[k];
			}

		}

		public string ToString()
		{
			string s = "Query model for: " + query + "\n";
			foreach (var p in probabilities) {
				s += p.Key.Keyword + " " + p.Value.ToString("F8") + "\n";
			}
			return s;
		}

	}

}
