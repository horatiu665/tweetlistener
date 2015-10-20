using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class MailHelperViewModel : ViewModelBase
	{
		private MailHelper mailHelper;
		
		public MailHelper MailHelper
		{                                                
		    get                                          
		    {                                            
		        return mailHelper;                  
		    }                                            
		                                                 
		    set                                          
		    {                                            
		        mailHelper = value;                 
		    }                                            
		}

        public override string Name
        {
            get
            {
                return "Mail Helper";
                throw new NotImplementedException();
            }
        }

        public MailHelperViewModel(LogViewModel log, StreamViewModel stream)
		{
			// create new model
			mailHelper = new MailHelper(log.Log, stream.Stream);
		}
	}
}