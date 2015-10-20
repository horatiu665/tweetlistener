using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class DatabaseViewModel : ViewModelBase
	{
		private Database database;
		
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

        public override string Name
        {
            get
            {
                return "Database";
            }
        }

        public DatabaseViewModel(LogViewModel log)
		{
			// create new model
			database = new Database(log.Log);
		}
	}
}