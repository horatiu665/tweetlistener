using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi.Core.Interfaces;

namespace TweetListener2.Systems
{

    public class TweetData : INotifyPropertyChanged
    {
        public ITweet tweet;

        public enum Sources
        {
            Stream,
            Rest,
            Unknown
        }

        // where did we find this tweet?
        public Sources source;

        // which cycle yielded this tweet?
        public int gatheringCycle;

        // which _expansion cycle first found this tweet?
        public int firstExpansion;

        // how many times was this tweet found after being found once and processed?
        public int howManyTimesFound = 1;

        /// <summary>
        /// returns list of hashtags as found raw in ITweet.
        /// </summary>
        /// <returns></returns>
        public List<string> GetHashtags()
        {
            return tweet.Hashtags.Select(he => he.Text).ToList();
        }

        /// <summary>
        /// does this tweet contain the hashtag tag?
        /// </summary>
        /// <param name="tag">hashtag to contain</param>
        /// <returns></returns>
        public bool ContainsHashtag(string tag)
        {
            if (tweet.Hashtags.Count == 0) return false;
            var withoutHash = tag.IndexOf("#") == 0 ? tag.Substring(1) : tag;
            return tweet.Hashtags.Select(he => he.Text.ToLower()).Contains(withoutHash);
        }

        public TweetData(ITweet t, Sources source, int gatheringCycle, int firstExpansion)
        {
            tweet = t;
            this.source = source;
            this.gatheringCycle = gatheringCycle;
            this.firstExpansion = firstExpansion;

            this.retweetCount = t.RetweetCount;

        }

        public DateTime Date
        {
            get
            {
                return tweet.CreatedAt;
            }
        }

        public string Tweet
        {
            get
            {
                return tweet.Text;
            }
        }

        public long Id
        {
            get
            {
                return tweet.Id;
            }
        }

        public string Lang
        {
            get
            {
                return tweet.Language.ToString();
            }
        }

        public string User
        {
            get
            {
                return tweet.CreatedBy.Name;
            }
        }

        public long UserId
        {
            get
            {
                return tweet.CreatedBy.Id;
            }
        }

        public string Source
        {
            get
            {
                return source.ToString();
            }
        }




        private int retweetCount = 0;
        /// <summary>
        /// counts how many times the tweet was posted again with a different link 
        /// (most likely by a tweet bot)
        /// </summary>
        private int duplicateCount = 0;


        /// <summary>
        /// Retweet count, counted manually
        /// beginning at twitter data when TweetData is constructed
        /// and counting every time the tweet id is found in another tweet's retweet id
        /// </summary>
        public int RetweetCount
        {
            get
            {
                return retweetCount;
            }
            set
            {
                retweetCount = value;
                //if (PropertyChanged != null) {
                //	PropertyChanged(this, new PropertyChangedEventArgs("RetweetCount"));
                //}
            }
        }

        public List<string> Links
        {
            get
            {
                return tweet.Urls.Select(urlEntity => urlEntity.URL).ToList();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
    }
}
