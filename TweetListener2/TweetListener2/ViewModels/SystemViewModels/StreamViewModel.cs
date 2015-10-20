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

        public bool StreamRunning
        {
            get
            {
                return stream.StreamRunning;
            }
        }

        public bool CountersOn
        {
            get
            {
                return stream.CountersOn;
            }
            set
            {
                stream.CountersOn = value;
            }
        }

        public bool LogEveryJson
        {
            get
            {
                return stream.LogEveryJson;
            }
            set
            {
                stream.LogEveryJson = value;
            }
        }

        public override string Name
        {
            get
            {
                return "Stream";
            }
        }

        internal void StartStream()
        {
            stream.Start();
        }

        internal void RestartStream()
        {
            stream.Stop();
            Task.Factory.StartNew(() => {
                stream.Log.Output("Separate thread waiting to start stream after it stops");

                // wait until stream stops
                while (stream.StreamRunning) {
                    ;
                }
                stream.Start();

            });
        }

        internal void StopStream()
        {
            stream.Stop();
        }
    }
}
