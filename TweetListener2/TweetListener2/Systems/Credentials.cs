using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;
using Newtonsoft.Json.Linq;
using Tweetinvi.Core.Credentials;

namespace TweetListener2.Systems
{
    public class Credentials
    {

        /// <summary>
        /// Which log to use for debug?
        /// </summary>
        private Log log;

        public Log Log
        {
            get
            {
                return log;
            }

            set
            {
                log = value;
            }
        }

        /// <summary>
        /// Active credentials, should remain private, current creds can be accessed using Tweetinvi.Auth.ApplicationCredentials instead.
        /// </summary>
        private List<string> credentials;

        /// <summary>
        /// hardcoded default credentials that can be set using the interface
        /// </summary>
        private static List<List<string>> defaults = new List<List<string>>() {
			// Testing3473487384
			new List<string>() {"2504893657-b30BlFnSdCKo42LFIyWKbywseTq2PyG0StdpKp6", "JDCAI46G4qDWHPYLjfhuM9iDll53wgRwXZKhcMkw84dwi", "s3QKQous2rgkpglkSTRHQz9dw", "t9cZGT3Rcheh8742LVZHaIc5uvLsSXSGvqUb3NIGr9WMt097IH"                    },
			// Testing2348276347
			new List<string>() {"2504893657-smZtLp98qZ7e461o1qUk361Hx7DX01rEv6c94JA", "L4Q0jEjexHFUR2GHOMi3T4iTERARUB3AuOzhQTDOvlEx4", "iJwYrZasX8aGU0iiHCbmYajCv", "FJPhwIHbXGX0XKbzioNlbl3UEViNRXu3Ny2BXFCzQ0eY4N231G"},

        };

        /// <summary>
        /// The available credentials for the application
        /// </summary>
        public static List<List<string>> Defaults
        {
            get
            {
                return defaults;
            }

            private set
            {
                defaults = value;
            }
        }

        /// <summary>
        /// constructor, initializes default credentials from file and initializes Auth.ApplicationCredentials
        /// </summary>
        /// <param name="log"></param>
        public Credentials(Log log)
        {
            this.Log = log;
            // reads list of credentials from file, saves them in private field for use in UI
            ReadCredentialsDefaults();
            // init credentials to the default creds, for the sake of timewaste
            TwitterCredentialsInit();

        }

        /// <summary>
        /// returns one of the credentials in the list
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public List<string> GetDefaults(int i)
        {
            i = i % Defaults.Count;
            return Defaults[i];

        }

        /// <summary>
        /// Gets List of List<string>[4] of credentials, from json data in "config.ini". 
        /// Returns null if file not found or gives error.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void ReadCredentialsDefaults(string path = "config.ini")
        {
            if (File.Exists("config.ini")) {
                try {
                    var jsonRead = JArray.Parse(File.ReadAllText("config.ini"));
                    // jsonRead should contain an array of credential objects
                    #region // format for credentials
                    /*
					[
						{
						  "Access_Token": "blabla",
						  "Access_Token_Secret": "blabla",
						  "Consumer_Key": "blabla",
						  "Consumer_Secret": "blabla"
						},
						{
						  "Access_Token": "blabla",
						  "Access_Token_Secret": "blabla",
						  "Consumer_Key": "blabla",
						  "Consumer_Secret": "blabla"
						}
					]
					*/
                    #endregion

                    List<List<string>> credentials = new List<List<string>>();
                    foreach (JObject jsonCredential in jsonRead) {
                        // jsonCredential is the credential format { "Access_Token":"", "Access_Token_Secret":"", "Consumer_Key":"", "Consumer_Secret":"" }
                        credentials.Add(new List<string>() {
                            jsonCredential["Access_Token"].ToString(),
                            jsonCredential["Access_Token_Secret"].ToString(),
                            jsonCredential["Consumer_Key"].ToString(),
                            jsonCredential["Consumer_Secret"].ToString()
                        });
                    }
                    Defaults = credentials;
                }
                catch {
                    Log.Output("Credentials file corrupted or incorrect format. File must contain an array of credential objects, with the format "
                        + "{ \"Access_Token\":\"\", \"Access_Token_Secret\":\"\", \"Consumer_Key\":\"\", \"Consumer_Secret\":\"\" }");
                }
            }
        }

        /// <summary>
        /// Fired when credentials change
        /// </summary>
        public event Action<List<string>> CredentialsChange;

        /// <summary>
        /// Sets credentials
        /// </summary>
        /// <param name="index"></param>
        public void SetCredentials(int index)
        {
            TwitterCredentialsInit(GetDefaults(index));
            CurrentCredentialsIndex = index % Defaults.Count;
        }

        /// <summary>
        /// maintains index of current credentials, out of defaults, and provides it for the view
        /// </summary>
        int currentCredentialsIndex = -1;
        public int CurrentCredentialsIndex
        {
            get
            {
                return currentCredentialsIndex;
            }
            private set
            {
                currentCredentialsIndex = value;
            }
        }

        public int Count
        {
            get
            {
                return defaults.Count;
            }
        }

        public ITwitterCredentials GetCurrentCredentials()
        {
            return Auth.Credentials;
        }




        /// <summary>
        /// set twitter credentials. Can use list of strings (custom new credentials), or default credentials.
        /// </summary>
        private void TwitterCredentialsInit(List<string> creds = null)
        {
            // if creds == null, init was called for reset (from file or hardcoded).
            if (creds == null) {

                // default creds
                credentials = Defaults[0];
                CurrentCredentialsIndex = 0;

                if (!File.Exists("config.ini")) {

                    var jarray = new JArray();
                    foreach (var def in Defaults) {

                        var jsonWrite = new JObject();
                        jsonWrite.Add("Access_Token",
                            new JValue(def[0]));
                        jsonWrite.Add("Access_Token_Secret",
                            new JValue(def[1]));
                        jsonWrite.Add("Consumer_Key",
                            new JValue(def[2]));
                        jsonWrite.Add("Consumer_Secret",
                            new JValue(def[3]));
                        jarray.Add(jsonWrite);

                    }
                    StreamWriter sw = new StreamWriter("config.ini");
                    sw.Write(jarray.ToString());
                    sw.Close();
                }

            } else {
                // set credentials to creds (was called using GetDefaults(i)) - BEWARE OF making this function public and calling it without setting the proper currentCredentialsIndex
                if (creds[0] != null && creds[1] != null && creds[2] != null && creds[3] != null) {
                    credentials = new List<string>(creds);
                }
            }

            // sets application credentials, however it is possible that since we are running multiple streams per application, that the credentials for the entire application are actually only one set, even though we are running multiple "applications" which are actually just multiple objects. Static is static dammit
            SetCurrentCredentialsForThread();


            if (CredentialsChange != null) {
                CredentialsChange(credentials);
            }

        }

        /// <summary>
        /// Must be called by each stream as it starts, because otherwise the stupid application thinks it has the same credentials as the other parallel TweetListeners. (static be static)
        /// relevant thread:
        /// https://twittercommunity.com/t/sudden-increase-in-420-exceeded-connection-limit-for-user-responses-from-streaming-api/15101
        /// </summary>
        public void SetCurrentCredentialsForThread()
        {
            Auth.SetCredentials(new TwitterCredentials(
                // "Consumer_Key"
                credentials[2],
                // "Consumer_Secret"
                credentials[3],
                // "Access_Token"
                credentials[0],
                // "Access_Token_Secret"
                credentials[1]
                ));
        }

    }
}
