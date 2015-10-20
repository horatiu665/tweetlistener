using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class QueryExpansionViewModel : ViewModelBase
	{
		private QueryExpansion queryExpansion;
		
		public QueryExpansion QueryExpansion
		{                                                
		    get                                          
		    {                                            
		        return queryExpansion;                  
		    }                                            
		                                                 
		    set                                          
		    {                                            
		        queryExpansion = value;                 
		    }                                            
		}

        public override string Name
        {
            get
            {
                return "Query expansion";
                throw new NotImplementedException();
            }
        }

        public QueryExpansionViewModel(LogViewModel log)
		{
			// create new model
			queryExpansion = new QueryExpansion(log.Log);
		}
	}
}