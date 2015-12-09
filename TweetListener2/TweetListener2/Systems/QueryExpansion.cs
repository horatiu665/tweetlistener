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

        public double TimeForLastOperation
        {
            get
            {
                return timeForLastOperation;
            }

            set
            {
                smoothTime = smoothTime * (1 - timeForLastOperationSmoothing) + value * timeForLastOperationSmoothing;
                timeForLastOperation = value;

            }
        }

        private double smoothTime = 0;
        float timeForLastOperationSmoothing = 0.01f;

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
                        ? "Expanding... estimated time left: " + TimeSpan.FromSeconds(OperationsLeft * smoothTime)
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

        /// <summary>
        /// Expands on keywords list, returns new keywords based on the ones that are "Used" within the keyword list provided, in tweets belonging in tweetPopulation list.
        /// </summary>
        /// <param name="keywords">keyword list (only expands on the "Used" ones)</param>
        /// <param name="tweetPopulation">expand upon this population</param>
        /// <returns>new list of expanded keywords, none of which belong to the provided keyword list</returns>
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
                TimeForLastOperation = 0.001f;
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
                TimeForLastOperation = 1f;
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
                TimeForLastOperation = 0.001f;
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

                TimeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;

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
                    TimeForLastOperation = 0.001f;
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
            TimeForLastOperation = 0.001f;
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
            QueryModel queryModel = new QueryModel(query, keywords, tweetPopulation, out tweetsReturnedByQueryModel, ExpandOnlyOnCooccurring);
            
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
                // optimize if we use cooccurring (less computation, less memory)
                if (ExpandOnlyOnCooccurring) {
                    if (keyword.LanguageModel == null) {
                        keyword.LanguageModel = new LanguageModel(keyword, keywords, tweetsReturnedByQueryModel, keywordCountsC, LanguageModel.SmoothingMethods.BayesianDirichlet, ExpandOnlyOnCooccurring, efronMu);
                    }
                } else {
                    if (keyword.LanguageModel == null) {
                        keyword.LanguageModel = new LanguageModel(keyword, keywords, tweetPopulation, keywordCountsC, LanguageModel.SmoothingMethods.BayesianDirichlet, ExpandOnlyOnCooccurring, efronMu);
                    }
                }
                if (forceStop) {
                    forceStop = false;
                    expanding = false;
                    return newList; ;
                }
                TimeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
                OperationsLeft--;
            }


            // calculate KL divergence for each hashtag vs query, and rank them. Only take N for ranking (N = arbitrary 25)
            OperationsLeft = keywords.Count;
            var rankedTags = keywords.OrderByDescending(kData => {
                initTime = DateTime.Now;
                double x;
                if (ExpandOnlyOnCooccurring) {
                    // when expanding only on cooccurring, we are assuming order is maintained for the keyword vocabulary, therefore we use the list of query/language model probabilitiesAssumingOrder rather than dictionaries
                    x = -LanguageModel.KlDivergence(
                        queryModel.probabilitiesAssumingOrder,
                        kData.LanguageModel.probabilitiesAssumingOrder);
                } else {
                    // see above comment
                    x = -LanguageModel.KlDivergence(
                        queryModel.probabilities.Values.ToList(),
                        kData.LanguageModel.probabilities.Values.ToList()
                    );
                }
                kData.LanguageModel.KldResult = x;
                TimeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
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
                    TimeForLastOperation = DateTime.Now.Subtract(initTime).TotalSeconds;
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


            for (int i = 0; i < Math.Min(rankedTags.Count, RankNHashtags); i++) {
                newList.Add(rankedTags[i]);
            }

            expanding = false;
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

        /// <summary>
        /// Expands in a clever way: selects cooccurring hashtags, expands using Efron with optimal (heuristic) settings, and returns a list of proposed expansion keywords.
        /// </summary>
        public KeywordDatabase.KeywordListClass ExpandThatQuery(KeywordDatabase keywordDatabase, TweetDatabase tweetDatabase)
        {

            // Steps for a clever query expansion (described in comments)
            var originalQuery = keywordDatabase.KeywordList.Where(kd => kd.UseKeyword);

            // 1. Do not delete or add any hashtags to the KeywordList because it will mess up the WPF data binding stuff. Instead, use internal memory for these operations and do not update actual lists at all.
            KeywordDatabase.KeywordListClass keywordListToExpand = new KeywordDatabase.KeywordListClass();
            keywordListToExpand.AddRange(originalQuery);

            // 2. naive expand 100% of all 1st generation cooccurring hashtags of the current query
            NaiveExpansionPercentage = 100;
            NaiveExpansionGenerations = 1;
            var newKeywords = ExpandNaive(originalQuery.ToList(), tweetDatabase.GetAllTweets().ToList());
            /// still expanding even though it might seem like we are done
            expanding = true;
            keywordListToExpand.AddRange(newKeywords);

            // 3. reset selection to the initial hashtags (set USE to false for all new hashtags)
            foreach (var k in keywordListToExpand) {
                k.UseKeyword = originalQuery.Contains(k);
            }

            // 4. clear language models for all keywords
            foreach (var k in keywordListToExpand) {
                k.LanguageModel = null;

            }

            // 5. set EfronMU based on expansion population (based on curve or some function)
            // 5.1 count how many tweets contain any of the hashtags in the query
            var tweetCount = tweetDatabase.GetAllTweets().Count(t => {
                foreach (var k in originalQuery) {
                    if (t.ContainsHashtag(k.Keyword)) {
                        return true;
                    }
                }
                return false;
            });
            // 5.2 based on tweetCount, set efronMu to an appropriate smoothing value based on heuristics
            if (tweetCount < 1000) {
                EfronMu = 50;
            } else if (tweetCount < 5000) {
                EfronMu = 100;
            } else if (tweetCount < 20000) {
                EfronMu = 200;
            } else if (tweetCount < 50000) {
                EfronMu = 500;
            } else if (tweetCount < 100000) {
                EfronMu = 1000;
            } else {
                EfronMu = 2000;
            }

            // 6. set suggestion count to default 25 (or maybe just show all suggestions with a max of 100 or something)
            RankNHashtags = 100;

            // 7. set ApplyFeedback and LogModels to false 
            ApplyFeedback = false;
            LogModels = false;
            // 7.1 set ExpandOnlyOnCooccurring to true because it optimizes LM calculation, and we also cleared all language models
            ExpandOnlyOnCooccurring = true;

            // 8. Finally expand.
            var efronExpanded = ExpandEfron(keywordListToExpand, tweetDatabase.GetAllTweets());

            // 9. Now we have the list of hashtags, we should probably provide a nice interface for them to be added. Let's instead return them as a list.
            return efronExpanded;
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
