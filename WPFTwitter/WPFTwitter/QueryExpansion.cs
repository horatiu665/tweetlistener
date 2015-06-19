using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace WPFTwitter
{
	public class QueryExpansion : INotifyPropertyChanged
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

		private bool expanding = false;
		public bool Expanding
		{
			get { return expanding; }
			set
			{
				expanding = value;
				OnPropertyChanged("ExpansionProgressLabel");
			}
		}
		private double timeForLastOperation = 0;
		private int operationsLeft = 0;
		public int OperationsLeft
		{
			get { return operationsLeft; }
			set
			{
				operationsLeft = value;
				OnPropertyChanged("ExpansionProgressLabel");
			}
		}
		public string ExpansionProgressLabel
		{
			get
			{
				return Expanding
						? "Expanding... estimated time left: " + OperationsLeft * timeForLastOperation + " seconds"
						: "Idle";
			}
		}

		public QueryExpansion(Log log)
		{
			this.log = log;
		}

		public List<KeywordDatabase.KeywordData> ExpandNaive(List<KeywordDatabase.KeywordData> keywords, List<TweetDatabase.TweetData> tweetPopulation)
		{
			log.Output("Expansion: NAIVE\n expanding query on " + tweetPopulation.Count + " tweets");

			Expanding = true;

			// find keywords from tweetCollection (and their count)
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

			Expanding = false;

			return keywords.Concat(newKeywordList).ToList();
		}

		public List<KeywordDatabase.KeywordData> ExpandEfron(
			KeywordDatabase.KeywordListClass keywords,
			List<TweetDatabase.TweetData> tweetPopulation)
		{
			expanding = true;
			log.Output("Expansion: EFRON\n expanding query on " + tweetPopulation.Count + " tweets");
			string s;
			// old test shit
			if (false) {
				foreach (var keyword in keywords) {
					if (keyword.UseKeyword) {
						LanguageModel lm = new LanguageModel(keyword, keywords, tweetPopulation, LanguageModel.SmoothingMethods.BayesianDirichlet, efronMu);
						s = "Language model for " + keyword.Keyword + ":\n";
						foreach (var kvp in lm.probabilities) {
							s += kvp.Value.ToString("F9") + "\t" + kvp.Key.Keyword + "; \n";
						}
						log.Output(s);
					}
				}
			}

			// generate query
			var query = "";
			foreach (var keyword in keywords) {
				if (keyword.UseKeyword) {
					query += keyword.Keyword + ",";
				}
			}

			// create query model
			QueryModel queryModel = new QueryModel(query, keywords, tweetPopulation);
			s = "query model for query:\n " + query + ":\n";
			foreach (var kvp in queryModel.probabilities) {
				s += kvp.Value.ToString("F9") + "\t" + kvp.Key.Keyword + "; \n";
			}
			log.Output(s);

			Dictionary<KeywordDatabase.KeywordData, LanguageModel> languageModels = new Dictionary<KeywordDatabase.KeywordData, LanguageModel>();
			OperationsLeft = keywords.Count;
			DateTime initTime;
			foreach (var keyword in keywords) {
				initTime = DateTime.Now;
				languageModels[keyword] = new LanguageModel(keyword, keywords, tweetPopulation, LanguageModel.SmoothingMethods.BayesianDirichlet, efronMu);
				s = "Language model for " + keyword.Keyword + ":\n";
				foreach (var kvp in languageModels[keyword].probabilities) {
					s += kvp.Value.ToString("F9") + "\t" + kvp.Key.Keyword + "; \n";
				}
				log.Output(s);
				timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
				OperationsLeft--;
			}

			OperationsLeft = languageModels.Count;
			
			// calculate KL divergence for each hashtag vs query, and rank them
			var rankedTags = languageModels.OrderByDescending(lm => {
				initTime = DateTime.Now;
				var x = -LanguageModel.KlDivergence(
					queryModel.probabilities.Values.ToList(),
					lm.Value.probabilities.Values.ToList()
				);
				lm.Value.KldResult = x;
				timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
				OperationsLeft--;
				return x;
			}).ToList();

			s = "List of ranked hashtags in descending order of their -KLD for the query:\n"
				+ queryModel.query + "\n";

			foreach (var r in rankedTags) {
				s += r.Value.KldResult.ToString("F8") + " " + r.Key.Keyword + "\n";
			}
			log.Output(s);

			return null;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		public void OnPropertyChanged(string pName)
		{
			if (PropertyChanged != null) {
				PropertyChanged(this, new PropertyChangedEventArgs(pName));
			}
		}
	}
}
