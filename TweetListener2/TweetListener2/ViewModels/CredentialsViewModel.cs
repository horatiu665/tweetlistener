using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using TweetListener2.Systems;

namespace TweetListener2.ViewModels
{
    public class CredentialsViewModel : ViewModelBase
    {
        Credentials credentials;

        public Credentials Credentials
        {
            get
            {
                return credentials;
            }

            private set
            {
                credentials = value;
            }
        }

        public Log Log
        {
            get
            {
                return Credentials.Log;
            }
            set
            {
                Credentials.Log = value;
            }
        }

        /// <summary>
        /// only for display
        /// </summary>
        public string SelectedCredentials
        {
            get
            {
                var ddd = Credentials.Defaults.FirstOrDefault(cred =>
                        cred[0] == Auth.ApplicationCredentials.AccessToken &&
                        cred[1] == Auth.ApplicationCredentials.AccessTokenSecret &&
                        cred[2] == Auth.ApplicationCredentials.ConsumerKey &&
                        cred[3] == Auth.ApplicationCredentials.ConsumerSecret
                    );
                if (ddd != null) {
                    var index = Credentials.Defaults.IndexOf(ddd);
                    if (index >= 0) {
                        return (index + 1).ToString() + "/" + Credentials.Defaults.Count;
                    }
                }
                return "other/" + Credentials.Defaults.Count;
            }
        }

        /// <summary>
        /// init viewModel
        /// </summary>
        public CredentialsViewModel()
        {
            // create new model
            Credentials = new Credentials();
        }

        public void ReadCredentialsFromFile(string path = "config.ini")
        {
            Credentials.ReadCredentialsDefaults(path);

        }
        
        public void SetCredentials(int index)
        {
            Credentials.SetCredentials(index);
        }

        public void SetCredentials(List<string> credentials)
        {
            Credentials.TwitterCredentialsInit(credentials);
        }
    }
}
