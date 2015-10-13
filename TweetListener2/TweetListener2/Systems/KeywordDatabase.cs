using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{
    public class KeywordDatabase
    {
        Log log;

        public bool ContinuousUpdate { get; set; }

        public List<string> GetUsableKeywords()
        {
            return keywordList.Where(kd => kd.UseKeyword).Select(kd => kd.Keyword).ToList();
        }

        public KeywordDatabase(Log log)
        {
            this.log = log;

            //keywordList.CollectionChanged += (s, a) => {
            //	if (keywordList != null) {
            //		keywordList.ClearLanguageModels();
            //	}
            //};
        }

        /// <summary>
        /// stores a list of the current keywords being looked for
        /// </summary>
        private KeywordListClass keywordList = new KeywordListClass();
        public KeywordListClass KeywordList
        {
            get
            {
                return keywordList;
            }
            set
            {
                // should not set {} an observable collection because it breaks the binding.
                // instead should only mess around with private value. unoptimally: clear list and add each element.
                keywordList = value;
            }
        }


        public class KeywordListClass : RangeObservableCollection<KeywordData>
        {
            public void Set(ObservableCollection<KeywordData> newList)
            {
                this.Clear();
                this.AddRange(newList);
            }

            public void Set(List<KeywordData> newList)
            {
                this.Clear();
                this.AddRange(newList);
            }

            public void Set(IEnumerable<KeywordData> newList)
            {
                this.Clear();
                this.AddRange(newList);
            }

            /// <summary>
            /// updates keywordlist based on list of tweets
            /// </summary>
            public void UpdateCount(IEnumerable<TweetData> tweets)
            {
                foreach (var kData in this) {
                    try {
                        // count appearances in tweet
                        var c = tweets.Count(td => td.ContainsHashtag(kData.Keyword));

                        kData.Count = c;
                    }
                    catch { }
                }
            }

            public void ClearLanguageModels()
            {
                foreach (var k in this) {
                    k.LanguageModel = null;
                }
            }
        }


    }

}
