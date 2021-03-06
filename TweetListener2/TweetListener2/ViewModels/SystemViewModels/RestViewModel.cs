using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class RestViewModel : ViewModelBase
	{
		private Rest rest;
		
		public Rest Rest
		{                                                
		    get                                          
		    {                                            
		        return rest;                  
		    }                                            
		                                                 
		    set                                          
		    {                                            
		        rest = value;                 
		    }                                            
		}

        public override string Name
        {
            get
            {
                return "Rest";
                throw new NotImplementedException();
            }
        }

        public RestViewModel(DatabaseViewModel database, LogViewModel log, TweetDatabaseViewModel tweetDb, CredentialsViewModel credentials)
		{
			// create new model
			rest = new Rest(database.Database, log.Log, tweetDb.TweetDatabase, credentials.Credentials);
		}
	}
}