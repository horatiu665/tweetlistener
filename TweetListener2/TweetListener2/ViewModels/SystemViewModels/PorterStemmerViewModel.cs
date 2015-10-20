using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
	public class PorterStemmerViewModel : ViewModelBase
	{
		private PorterStemmer porterStemmer;
		
		public PorterStemmer PorterStemmer
		{                                                
		    get                                          
		    {                                            
		        return porterStemmer;                  
		    }                                            
		                                                 
		    set                                          
		    {                                            
		        porterStemmer = value;                 
		    }                                            
		}

        public override string Name
        {
            get
            {
                return "Porter stemmer";
            }
        }

        public PorterStemmerViewModel()
		{
			// create new model
			porterStemmer = new PorterStemmer();
		}
	}
}