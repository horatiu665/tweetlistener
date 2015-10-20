using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class KeywordDatabaseViewModel : ViewModelBase
	{
		private KeywordDatabase keywordDatabase;

        public KeywordDatabase KeywordDatabase
        {
            get
            {
                return keywordDatabase;
            }

            set
            {
                keywordDatabase = value;
            }
        }

        public override string Name
        {
            get
            {
                return "Keyword Database";

            }
        }

        public KeywordDatabaseViewModel(LogViewModel log)
		{
			// create new model
			KeywordDatabase = new KeywordDatabase(log.Log);
		}

    }
}