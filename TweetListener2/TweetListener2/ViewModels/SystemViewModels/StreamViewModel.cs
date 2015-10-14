using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class StreamViewModel : ViewModelBase
    {
        private Stream stream;

        public Stream Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        public StreamViewModel(DatabaseViewModel database, LogViewModel log, RestViewModel rest, KeywordDatabaseViewModel keywordDatabase, TweetDatabaseViewModel tweetDatabase)
        {
            Stream = new Stream(database.Database, log.Log, rest.Rest, keywordDatabase.KeywordDatabase, tweetDatabase.TweetDatabase);
        }

        public bool IsRunning
        {
            get
            {
                throw new NotImplementedException();
                //return false;
            }
        }

        public string Tracks
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
