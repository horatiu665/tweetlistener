using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;
using Newtonsoft.Json.Linq;
using System.ComponentModel;

namespace WPFTwitter
{
	public class Credentials : INotifyPropertyChanged
	{
		private Log log;

		public Credentials(Log log)
		{
			this.log = log;
			// reads list of credentials from file, saves them in private field for use in UI
			ReadCredentialsDefaults();
			// init credentials to the default creds, for the sake of timewaste
			TwitterCredentialsInit();

			CredentialsChange += (creds) => {
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs("SelectedCredentials"));
			};
		}

		private List<string> credentials = new List<string>() { "", "", "", "" };

		/// <summary>
		/// hardcoded default credentials that can be set using the interface
		/// </summary>
		private List<List<string>> defaults = new List<List<string>>() {
			// Testing3473487384
			new List<string>() {"2504893657-b30BlFnSdCKo42LFIyWKbywseTq2PyG0StdpKp6", "JDCAI46G4qDWHPYLjfhuM9iDll53wgRwXZKhcMkw84dwi", "s3QKQous2rgkpglkSTRHQz9dw", "t9cZGT3Rcheh8742LVZHaIc5uvLsSXSGvqUb3NIGr9WMt097IH"					},
			// Testing2348276347
			new List<string>() {"2504893657-smZtLp98qZ7e461o1qUk361Hx7DX01rEv6c94JA", "L4Q0jEjexHFUR2GHOMi3T4iTERARUB3AuOzhQTDOvlEx4", "iJwYrZasX8aGU0iiHCbmYajCv", "FJPhwIHbXGX0XKbzioNlbl3UEViNRXu3Ny2BXFCzQ0eY4N231G"},
			
		};

		public string SelectedCredentials
		{
			get
			{
				var ddd = defaults.FirstOrDefault(cred =>
						cred[0] == TwitterCredentials.Credentials.AccessToken &&
						cred[1] == TwitterCredentials.Credentials.AccessTokenSecret &&
						cred[2] == TwitterCredentials.Credentials.ConsumerKey &&
						cred[3] == TwitterCredentials.Credentials.ConsumerSecret
					);
				if (ddd != null) {
					var index = defaults.IndexOf(ddd);
					if (index >= 0) {
						return (index + 1).ToString() + "/" + defaults.Count;
					}
				}
				return "other/" + defaults.Count;
			}
		}

		public List<string> GetDefaults(int i)
		{
			i = i % defaults.Count;
			return defaults[i];

		}

		/// <summary>
		/// Gets List of List<string>[4] of credentials, from json data in "config.ini". 
		/// Returns null if file not found or gives error.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private void ReadCredentialsDefaults(string path = "config.ini")
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
					defaults = credentials;
				}
				catch {
					log.Output("Credentials file corrupted or incorrect format. File must contain an array of credential objects, with the format "
						+ "{ \"Access_Token\":\"\", \"Access_Token_Secret\":\"\", \"Consumer_Key\":\"\", \"Consumer_Secret\":\"\" }");
				}
			}
		}

		public event Action<List<string>> CredentialsChange;

		/// <summary>
		/// set twitter credentials for the app
		/// </summary>
		public void TwitterCredentialsInit(List<string> creds = null)
		{
			// if creds == null, init was called for reset (from file or hardcoded).
			if (creds == null) {

				// default creds
				credentials = defaults[0];

				if (!File.Exists("config.ini")) {

					var jarray = new JArray();
					foreach (var def in defaults) {

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
				// set credentials to creds
				if (creds[0] != null && creds[1] != null && creds[2] != null && creds[3] != null) {
					credentials = new List<string>(creds);
				}
			}

			// not application credentials because not supported until Tweetinvi 0.9.9 (current is 0.9.7)
			TwitterCredentials.SetCredentials(TwitterCredentials.CreateCredentials(
				// "Access_Token"
				credentials[0],
				// "Access_Token_Secret"
				credentials[1],
				// "Consumer_Key"
				credentials[2],
				// "Consumer_Secret"
				credentials[3]
			));

			if (CredentialsChange != null) {
				CredentialsChange(credentials);
			}

		}


		public event PropertyChangedEventHandler PropertyChanged;
	}
}
