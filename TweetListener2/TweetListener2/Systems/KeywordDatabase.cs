using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{
    public class KeywordDatabase
    {
        Log log;
        Database database;

        public bool ContinuousUpdate { get; set; }

        public List<string> GetUsableKeywords()
        {
            return keywordList.Where(kd => kd.UseKeyword).Select(kd => kd.Keyword).ToList();
        }

        public KeywordDatabase(Log log, Database database)
        {
            this.Log = log;
            this.Database = database;

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

        public Database Database
        {
            get
            {
                return database;
            }

            set
            {
                database = value;
            }
        }

        public void SaveToTextFile()
        {
            var path = "Keywords/" + database.DatabaseTableName + "_keywords.txt";
            Directory.CreateDirectory("Keywords");

            StreamWriter sw = new StreamWriter(path, false);
            for (int i = 0; i < keywordList.Count; i++) {
                var s = keywordList[i].Keyword + "," + (keywordList[i].UseKeyword ? "1" : "0");

                sw.WriteLine(s);
            }
            sw.Close();

            Log.Output("Saved keyword list to " + path);

        }

        public void LoadFromTextFile()
        {
            var path = "Keywords/" + database.DatabaseTableName + "_keywords.txt";
            if (File.Exists(path)) {
                StreamReader sr = new StreamReader(path);
                string s;
                while (!sr.EndOfStream) {
                    s = sr.ReadLine();
                    var sData = s.Split(",".ToCharArray());
                    if (!KeywordList.Any(kd => kd.Keyword == sData[0])) {
                        keywordList.Add(new KeywordData(sData[0], 0));
                        keywordList.Last().UseKeyword = (sData[1] == "1");
                    }
                }
                sr.Close();
            } else {
                Log.Output("KeywordDatabase.LoadFromTextFile(): File at " + path + " does not exist, no keywords were loaded");
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
