using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{
    /// <summary>
    /// Model. Keeps track of all the systems: Streams, Rest, Databases, Logs, etc.
    /// </summary>
    public class SystemManager
    {
        public List<Stream> streams = new List<Stream>();
        public List<Rest> rests = new List<Rest>();
        public List<Credentials> credentials = new List<Credentials>();
        public List<Database> databases = new List<Database>();
        public List<Log> logs = new List<Log>();
        public List<KeywordDatabase> keywordDatabases = new List<KeywordDatabase>();
        public List<TweetDatabase> tweetDatabases = new List<TweetDatabase>();
        public List<QueryExpansion> queryExpansions = new List<QueryExpansion>();
        public List<PorterStemmer> porterStemmers = new List<PorterStemmer>();
        public List<MailHelper> mailHelpers = new List<MailHelper>();

    }
}
