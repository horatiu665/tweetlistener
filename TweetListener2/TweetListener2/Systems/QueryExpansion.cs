using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace TweetListener2.Systems
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
                Log.Output("Naive expansion percentage set to " + value);
                naiveExpansionPercentage = value;
            }
        }

        private float efronMu = 2000;

        public float EfronMu
        {
            get { return efronMu; }
            set { efronMu = value; }
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

        private int rankNHashtags = 25;
        public int RankNHashtags
        {
            get { return rankNHashtags; }
            set { rankNHashtags = value; }
        }

        /// <summary>
        /// in seconds
        /// </summary>
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

        private bool expandOnAllKeywords = false;
        public bool ExpandOnAllKeywords
        {
            get { return expandOnAllKeywords; }
            set { expandOnAllKeywords = value; }
        }

        private int naiveExpansionGenerations = 0;
        public int NaiveExpansionGenerations
        {
            get { return naiveExpansionGenerations; }
            set { naiveExpansionGenerations = value; }
        }

        private bool expandOnlyOnCooccurring = false;
        public bool ExpandOnlyOnCooccurring
        {
            get { return expandOnlyOnCooccurring; }
            set { expandOnlyOnCooccurring = value; }
        }

        public bool LogModels { get; set; }

        private bool applyFeedback = false;
        public bool ApplyFeedback
        {
            get { return applyFeedback; }
            set { applyFeedback = value; }
        }

        public Log Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        public QueryExpansion(Log log)
        {
            this.Log = log;
        }

        public List<KeywordData> ExpandNaive(List<KeywordData> keywords, List<TweetData> tweetPopulation)
        {
            Log.Output("Expansion: NAIVE\n expanding query on " + tweetPopulation.Count + " tweets");

            Expanding = true;

            ///save their count, because we return top % of them anyway.
            List<KeyValuePair<string, int>> selectedKeywords = new List<KeyValuePair<string, int>>();
            // if generation == 0,  select the top % of all tweets (or if there are no used keywords to expand on)
            if (naiveExpansionGenerations == 0 || keywords.Count(k => k.UseKeyword) == 0) {

                // find all keywords from tweetCollection (and their count)
                Dictionary<string, int> keywordsInTweets = new Dictionary<string, int>();
                timeForLastOperation = 0.001f;
                OperationsLeft = tweetPopulation.Count;
                foreach (var t in tweetPopulation) {
                    foreach (var ht in t.GetHashtags()) {
                        var tag = ht.ToLower();
                        if (keywordsInTweets.Keys.Contains(tag)) {
                            keywordsInTweets[tag]++;
                        } else {
                            keywordsInTweets[tag] = 1;
                        }
                    }
                    if (forceStop) {
                        forceStop = false;
                        expanding = false;
                        return null;
                    }
                    OperationsLeft--;
                }

                // remove the ones that are already in the keywords list
                foreach (var k in keywords) {
                    var toRemove = k.Keyword;
                    toRemove = toRemove.Replace("#", "");
                    keywordsInTweets.Remove(toRemove);
                }

                // order by count somehow
                var orderedKeywords = keywordsInTweets.OrderByDescending(kvp => kvp.Value);
                // top % of all tweets.
                selectedKeywords = orderedKeywords.Take((int)Math.Ceiling(orderedKeywords.Count() * (naiveExpansionPercentage / 100f))).ToList();
            } else {
                var initTime = DateTime.Now;
                timeForLastOperation = 1f;
                // for each generation count, select the keywords which co-occur with the selected ones in the keywordsList, and subsequently, with the already selected ones.

                List<TweetData> tweetsContainingSelectedKeywords = new List<TweetData>();

                // list of keywords which are UseKeyword from keywordsList
                var usedKeywords = keywords.Where(k => k.UseKeyword).Select(k => k.Keyword);
                foreach (var k in usedKeywords) {
                    tweetsContainingSelectedKeywords.AddRange(tweetPopulation.Where(t => t.ContainsHashtag(k)));
                }

                // list of keywords which belong to tweets which contain any of the searched keywords
                //var containingUsed = keywordsInTweets.Where(kvp => tweetsContainingSelectedKeywords.Any(t => t.ContainsHashtag(kvp.Key))).OrderByDescending(kvp => kvp.Value);
                Dictionary<string, int> containingUsed = new Dictionary<string, int>();
                timeForLastOperation = 0.001f;
                OperationsLeft = tweetsContainingSelectedKeywords.Count;
                foreach (var t in tweetsContainingSelectedKeywords) {
                    foreach (var ht in t.GetHashtags()) {
                        var tag = ht.ToLower();
                        if (containingUsed.Keys.Contains(tag)) {
                            containingUsed[tag]++;
                        } else {
                            containingUsed[tag] = 1;
                        }
                    }
                    if (forceStop) {
                        forceStop = false;
                        expanding = false;
                        return null;
                    }
                    OperationsLeft--;
                }

                // remove the ones that are already in the keywords list
                foreach (var k in keywords) {
                    var toRemove = k.Keyword;
                    toRemove = toRemove.Replace("#", "");
                    containingUsed.Remove(toRemove);
                }

                // save only the top percent of those cooccurring keywords.
                selectedKeywords = containingUsed.OrderByDescending(kvp => kvp.Value).Take((int)Math.Ceiling(containingUsed.Count() * (naiveExpansionPercentage / 100f))).ToList();

                timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;

                for (int i = 0; i < naiveExpansionGenerations - 1; i++) {
                    //initTime = DateTime.Now; 

                    tweetsContainingSelectedKeywords.Clear();
                    foreach (var k in selectedKeywords) {
                        tweetsContainingSelectedKeywords.AddRange(tweetPopulation.Where(t => t.ContainsHashtag(k.Key)));
                    }
                    foreach (var k in usedKeywords) {
                        tweetsContainingSelectedKeywords.AddRange(tweetPopulation.Where(t => t.ContainsHashtag(k)));
                    }

                    // repeat process, but choose cooccurring with selectedKeywords instead of usedKeywords
                    //containingUsed = keywordsInTweets.Where(kvp => tweetsContainingSelectedKeywords.Any(t => t.ContainsHashtag(kvp.Key))).OrderByDescending(kvp => kvp.Value);
                    containingUsed.Clear();
                    timeForLastOperation = 0.001f;
                    OperationsLeft = tweetsContainingSelectedKeywords.Count;
                    foreach (var t in tweetsContainingSelectedKeywords) {
                        foreach (var ht in t.GetHashtags()) {
                            var tag = ht.ToLower();
                            if (containingUsed.Keys.Contains(tag)) {
                                containingUsed[tag]++;
                            } else {
                                containingUsed[tag] = 1;
                            }
                        }
                        if (forceStop) {
                            forceStop = false;
                            expanding = false;
                            return null;
                        }
                        OperationsLeft--;
                    }

                    // remove the ones that are already in the keywords list
                    foreach (var k in keywords) {
                        var toRemove = k.Keyword;
                        toRemove = toRemove.Replace("#", "");
                        containingUsed.Remove(toRemove);
                    }

                    selectedKeywords = containingUsed.OrderByDescending(kvp => kvp.Value).Take((int)Math.Ceiling(containingUsed.Count() * (naiveExpansionPercentage / 100f))).ToList();

                    //timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
                    //OperationsLeft--;
                }
            }



            selectedKeywords = selectedKeywords.Where(kvp => kvp.Key.Any(cha => cha != ' ')).ToList();

            var logOutput = true;
            if (logOutput) {
                var s = "";

                Log.Output(selectedKeywords.Count + " unique keywords after expansion");
            }

            // create new keywordData for the new keywords found
            var newKeywordList = new List<KeywordData>();
            timeForLastOperation = 0.001f;
            OperationsLeft = selectedKeywords.Count();
            foreach (var e in selectedKeywords) {
                var newKData = new KeywordData("#" + e.Key, 0);
                newKData.Count = e.Value;
                newKeywordList.Add(newKData);
                OperationsLeft--;
            }

            OperationsLeft = 0;
            Expanding = false;

            return newKeywordList;
        }

        /// <summary>
        /// returns new keywords
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="tweetPopulation"></param>
        /// <returns></returns>
        public KeywordDatabase.KeywordListClass ExpandEfron(KeywordDatabase.KeywordListClass keywords, List<TweetData> tweetPopulation)
        {
            KeywordDatabase.KeywordListClass newList = new KeywordDatabase.KeywordListClass();

            expanding = true;
            if (LogModels)
                Log.Output("Expansion: EFRON\n expanding query on " + tweetPopulation.Count + " tweets");

            // generate query
            var query = "";
            foreach (var keyword in keywords) {
                if (keyword.UseKeyword) {
                    query += keyword.Keyword + ",";
                }
            }

            if (query == "") {
                expanding = false;
                Log.Output("Query was empty. no expansion");
                return newList;
            }

            if (forceStop) {
                forceStop = false;
                expanding = false;
                return newList; ;
            }

            // create query model
            List<TweetData> tweetsReturnedByQueryModel;
            QueryModel queryModel = new QueryModel(query, keywords, tweetPopulation, out tweetsReturnedByQueryModel);

            if (LogModels) {
                var s = queryModel.ToString();
                Log.Output(s);
            }

            // count words in tweet collection
            var keywordCountsC = new Dictionary<string, int>();
            foreach (var t in tweetPopulation) {
                foreach (var ht in t.GetHashtags()) {
                    var tag = ht.ToLower();
                    if (keywordCountsC.Keys.Contains(tag)) {
                        keywordCountsC[tag]++;
                    } else {
                        keywordCountsC[tag] = 1;
                    }
                }
            }

            // make language model inside each keyword
            OperationsLeft = keywords.Count;
            DateTime initTime;
            foreach (var keyword in keywords) {
                initTime = DateTime.Now;
                if (ExpandOnlyOnCooccurring) {
                    if (keyword.LanguageModel == null) {
                        keyword.LanguageModel = new LanguageModel(keyword, keywords, tweetsReturnedByQueryModel, keywordCountsC, LanguageModel.SmoothingMethods.BayesianDirichlet, efronMu);
                    }
                } else {
                    if (keyword.LanguageModel == null) {
                        keyword.LanguageModel = new LanguageModel(keyword, keywords, tweetPopulation, keywordCountsC, LanguageModel.SmoothingMethods.BayesianDirichlet, efronMu);
                    }
                }
                if (forceStop) {
                    forceStop = false;
                    expanding = false;
                    return newList; ;
                }
                if (LogModels) {
                    Log.Output(keyword.LanguageModel.ToString());
                }
                timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
                OperationsLeft--;
            }


            // calculate KL divergence for each hashtag vs query, and rank them. Only take N for ranking (N = arbitrary 25)
            OperationsLeft = keywords.Count;
            var rankedTags = keywords.OrderByDescending(kData => {
                initTime = DateTime.Now;
                var x = -LanguageModel.KlDivergence(
                    queryModel.probabilities.Values.ToList(),
                    kData.LanguageModel.probabilities.Values.ToList()
                );
                kData.LanguageModel.KldResult = x;
                timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
                OperationsLeft--;
                return x;
            }).Take(rankNHashtags).ToList();

            if (forceStop) {
                forceStop = false;
                expanding = false;
                return newList; ;
            }

            // use feedback on query model

            // /////////////////////////////// FEEDBACK MODEL HERE /////////////////////////////
            if (ApplyFeedback) {
                // Theta_r = weights for all ranked elements (just to increase their weight slightly in the final formula)
                var ThetaR = new Dictionary<KeywordData, float>();

                // use IDF? we need to calculate inverse document frequency for this. Else, use uniform distribution.
                var IDF = false;
                foreach (var r in rankedTags) {
                    ThetaR[r] = (IDF ? 0f : 1 / (float)rankedTags.Count);
                }

                // non-ranked elements are zero, regardless if we use IDF or uniform.
                foreach (var k in keywords) {
                    if (rankedTags.All(kvp => kvp.Keyword != k.Keyword)) {
                        ThetaR[k] = 0f;
                    }
                }

                if (forceStop) {
                    forceStop = false;
                    expanding = false;
                    return newList; ;
                }

                // now we have the ThetaR, we can include it in a feedback loop.
                queryModel.ApplyFeedback(ThetaR, 0.2f);

                if (LogModels) {
                    var s = "After applying feedback:\n" + queryModel.ToString();
                    Log.Output(s);
                }

                // after queryModel updated with feedback, ranking should be performed again.
                OperationsLeft = keywords.Count;
                rankedTags = keywords.OrderByDescending(kData => {
                    initTime = DateTime.Now;
                    var x = -LanguageModel.KlDivergence(
                        queryModel.probabilities.Values.ToList(),
                        kData.LanguageModel.probabilities.Values.ToList()
                    );
                    kData.LanguageModel.KldResult = x;
                    timeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
                    OperationsLeft--;
                    return x;
                }).Take(rankNHashtags).ToList();

            }


            if (true) {
                var s = "List of ranked hashtags in descending order of their -KLD for the query:\n"
                    + queryModel.query + "\n";
                foreach (var r in rankedTags) {
                    s += r.LanguageModel.KldResult.ToString("F8") + " " + r.Keyword + "\n";
                }
                Log.Output(s);
            }


            for (int i = 0; i < Math.Min(rankedTags.Count, 25); i++) {
                newList.Add(rankedTags[i]);
            }

            return newList;
        }


        private bool forceStop = false;

        /// <summary>
        /// stops any ongoing expansion
        /// </summary>
        public void Stop()
        {
            forceStop = true;
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
