using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class TweetDatabaseViewModel : ViewModelBase
	{
		private TweetDatabase tweetDatabase;
		
		public TweetDatabase TweetDatabase
		{                                                
		    get                                          
		    {                                            
		        return tweetDatabase;                  
		    }                                            
		                                                 
		    set                                          
		    {                                            
		        tweetDatabase = value;                 
		    }                                            
		}

        public override string Name
        {
            get
            {
                return "Tweet Database";
                throw new NotImplementedException();
            }
        }

        public TweetDatabaseViewModel(DatabaseViewModel database)
		{
			// create new model
			tweetDatabase = new TweetDatabase(database.Database);
		}
	}
}