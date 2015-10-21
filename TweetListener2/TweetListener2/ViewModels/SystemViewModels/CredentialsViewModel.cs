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
        /// init viewModel
        /// </summary>
        public CredentialsViewModel(LogViewModel log)
        {
            // create new model
            Credentials = new Credentials(log.Log);
        }

        /// <summary>
        /// OLD SHIT FROM OLD PROJECT PLS DELETE AND FIX
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

        internal void SetCredentialsButton()
        {
            Log.Output("Set credentials not implemented.. pls fix");
        }

        public string AccessToken
        {
            get
            {
                return Tweetinvi.Auth.ApplicationCredentials.AccessToken;
            }
            set
            {
                Log.Output("Set credentials not implemented.. pls fix");

            }
        }

        public string AccessTokenSecret
        {
            get
            {
                return Tweetinvi.Auth.ApplicationCredentials.AccessTokenSecret;
            }
            set
            {
                Log.Output("Set credentials not implemented.. pls fix");

            }
        }

        public string ConsumerKey
        {
            get
            {
                return Tweetinvi.Auth.ApplicationCredentials.ConsumerKey;
            }
            set
            {
                Log.Output("Set credentials not implemented.. pls fix");

            }
        }

        public string ConsumerSecret
        {
            get
            {
                return Tweetinvi.Auth.ApplicationCredentials.ConsumerSecret;
            }
            set
            {
                Log.Output("Set credentials not implemented.. pls fix");

            }
        }

        public override string Name
        {
            get
            {
                return "Credentials";
            }
        }

        /// <summary>
        /// TODO: instead of int, create a credential atom class, 
        /// that contains a list of string (the four credential strings) 
        /// and a bool which specifies if the cred is currently used by some stream or is available for use
        /// </summary>
        public List<int> CredentialsOptions
        {
            get
            {
                return Credentials.Defaults.Select(cList => Credentials.Defaults.IndexOf(cList)).ToList();
            }
        }

        int credIndex;

        /// <summary>
        /// TODO: instead of int, create a credential atom class, 
        /// that contains a list of string (the four credential strings) 
        /// and a bool which specifies if the cred is currently used by some stream or is available for use
        /// </summary>
        public int SelectedItem
        {
            get
            {
                return credentials.CurrentCredentialsIndex;
            }
            set
            {
                credIndex = value;
                credentials.SetCredentials(credIndex);
                // update properties for credential view... access key and all
            }
        }

        public void ReadCredentialsFromFile(string path = "config.ini")
        {
            Credentials.ReadCredentialsDefaults(path);

        }

        public void SetCredentials(int index)
        {
            Credentials.SetCredentials(index);
        }
        
    }
}
