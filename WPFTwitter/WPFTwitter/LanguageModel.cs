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
		public LanguageModel(KeywordDatabase.KeywordData word_i, KeywordDatabase.KeywordListClass vocabulary, TweetDatabase.TweetList tweetCollection, SmoothingMethods smoothingMethod, float smoothingMu = 2000f)
		{
			keyword = word_i;
			
			// use maximum likelihood estimator?
			if (smoothingMethod == SmoothingMethods.MLE) {
				MLE(vocabulary, tweetCollection, ref probabilities);
			} else {
				BayesianSmoothing(vocabulary, tweetCollection, ref probabilities, smoothingMu);
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
		/// Maximum likelihood estimator. Simplest language model, returns probability(i) = count of word(i) in D / total word count of D
		/// where D is set of tweets returned by query, in this case just one keyword.
		/// Calculates MLE in probs array (probabilities of the other words appearing along with this one).
		/// </summary>
		/// <param name="vocabulary"></param>
		/// <param name="tweetCollection"></param>
		void MLE(KeywordDatabase.KeywordListClass vocabulary, TweetDatabase.TweetList tweetCollection, ref Dictionary<KeywordDatabase.KeywordData, float> probs)
		{
			probs.Clear();
			
			// D = docs containing word_i
			var tweetsContainingHashtag = tweetCollection.Where(td => td.ContainsHashtag(keyword.Keyword)).ToList();
			
			// num words in D
			var numWordsInSample = 0;
			for (int i = 0; i < tweetsContainingHashtag.Count; i++) {
				numWordsInSample += tweetsContainingHashtag[i].tweet.Hashtags.Count;
			}
			
			// compute probability for each k in vocabulary to appear in a tweet from tweetCollection together with word_i
			foreach (var k in vocabulary) {

				var word_k = k.Keyword;
				if (word_k.Contains("#")) {
					word_k = word_k.Substring(1);
				}

				// maxmimum likelihood of K occurring along with word_i in a tweet is:
				// the number of times it appears in D
				// div. by the amount of words in D

				// num appearances of k in D
				var numAppearancesOfK = 0;
				for (int i = 0; i < tweetsContainingHashtag.Count; i++) {
					var tweet = tweetsContainingHashtag[i].tweet;

					// count occurrences of word_k in tweetText
					numAppearancesOfK += tweet.Hashtags.Count(h => h.Text.ToLower() == word_k);

				}

				// max likelihood 
				var maxLikelihoodOfK = numAppearancesOfK / (float)numWordsInSample;

				probs.Add(k, maxLikelihoodOfK);
			}

			// MLE calculated in the probs[] array.

		}

		
		/// <summary>
		/// Bayesian smoothing with Dirichlet priors, based on Efron and other sources. Details in paper.
		/// </summary>
		/// <param name="probs"></param>
		/// <param name="N"></param>
		/// <param name="smoothingConstantMu"></param>
		/// <returns></returns>
		void BayesianSmoothing(KeywordDatabase.KeywordListClass vocabulary, TweetDatabase.TweetList tweetCollection, ref Dictionary<KeywordDatabase.KeywordData, float> probabilities, float smoothingConstantMu)
		{
			probabilities.Clear();

			// D = docs containing word_i
			var tweetsContainingHashtag = tweetCollection.Where(td => td.ContainsHashtag(keyword.Keyword)).ToList();

			// num words in D
			var numWordsInSample = 0;
			for (int i = 0; i < tweetsContainingHashtag.Count; i++) {
				numWordsInSample += tweetsContainingHashtag[i].tweet.Hashtags.Count;
			}

			var numWordsInCollection = 0;
			for (int i = 0; i < tweetCollection.Count; i++) {
				numWordsInCollection += tweetCollection[i].tweet.Hashtags.Count;
			}

			// for each, use formula in paper, or see reference:
			// C. Zhai and J. Lafferty, "A Study of Smoothing Methods for Language Models Applied to Information Retrieval," ACM Transactions on Information Systems, vol. 22, no. 2, pp. 179-214, 2004.
			foreach (var k in vocabulary) {

				var word_k = k.Keyword;
				if (word_k.Contains("#")) {
					word_k = word_k.Substring(1);
				}

				// num appearances of k in D
				var countWordKInD = 0;
				for (int i = 0; i < tweetsContainingHashtag.Count; i++) {
					var tweet = tweetsContainingHashtag[i].tweet;

					// count occurrences of word_k in tweetText
					countWordKInD += tweet.Hashtags.Count(h => h.Text.ToLower() == word_k);

				}

				// num appearances of k in C
				var countWordKInC = 0;
				for (int i = 0; i < tweetCollection.Count; i++) {
					var tweet = tweetCollection[i].tweet;

					// count occurrences of word_k in tweetText
					countWordKInC += tweet.Hashtags.Count(h => h.Text.ToLower() == word_k);

				}

				// smooth prob
				var smoothProb = (countWordKInD + smoothingConstantMu * countWordKInC / numWordsInCollection) / ((float)numWordsInSample + smoothingConstantMu);

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
				if (probability2[i] != 0) {
					sum += (probability1[i] * Math.Log(probability1[i] / probability2[i]));
				}
			}
			return sum;
		}



	}
}
