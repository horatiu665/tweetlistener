using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Tweetinvi;

namespace WPFTwitter
{
	public class Credentials
	{

		private List<string> credentials = new List<string>() { "", "", "", "" };

		public event Action<List<string>> CredentialsChange;

		/// <summary>
		/// set twitter credentials for the app
		/// </summary>
		public void TwitterCredentialsInit(List<string> creds = null)
		{
			// if creds == null, init was called for reset (from file or hardcoded).
			if (creds == null) {

				// if .ini file found, take creds from there. else hardcode them.
				bool credsFoundInFile = false;
				if (File.Exists("config.ini")) {
					var jsonRead = Newtonsoft.Json.Linq.JObject.Parse(File.ReadAllText("config.ini"));
					credentials = new List<string>() {
						jsonRead["Access_Token"].ToString(),
						jsonRead["Access_Token_Secret"].ToString(),
						jsonRead["Consumer_Key"].ToString(),
						jsonRead["Consumer_Secret"].ToString()
					};
					if (credentials[0] != null && credentials[1] != null && credentials[2] != null && credentials[3] != null) {
						credsFoundInFile = true;
					}
				}

				if (!credsFoundInFile) {
					// HARD CODE the creds
					credentials = new List<string>() {
						"2504893657-b30BlFnSdCKo42LFIyWKbywseTq2PyG0StdpKp6",
						"JDCAI46G4qDWHPYLjfhuM9iDll53wgRwXZKhcMkw84dwi",
						"s3QKQous2rgkpglkSTRHQz9dw",
						"t9cZGT3Rcheh8742LVZHaIc5uvLsSXSGvqUb3NIGr9WMt097IH"
					};

					var jsonWrite = new Newtonsoft.Json.Linq.JObject();
					jsonWrite.Add("Access_Token",
						new Newtonsoft.Json.Linq.JValue(credentials[0]));
					jsonWrite.Add("Access_Token_Secret",
						new Newtonsoft.Json.Linq.JValue(credentials[1]));
					jsonWrite.Add("Consumer_Key",
						new Newtonsoft.Json.Linq.JValue(credentials[2]));
					jsonWrite.Add("Consumer_Secret",
						new Newtonsoft.Json.Linq.JValue(credentials[3]));
					StreamWriter sw = new StreamWriter("config.ini");
					sw.Write(jsonWrite.ToString());
					sw.Close();
				}

				if (CredentialsChange != null) {
					CredentialsChange(credentials);
				}
			} else {
				// set credentials to creds
				if (creds[0] != null && creds[1] != null && creds[2] != null && creds[3] != null) {
					credentials = new List<string>(creds);
				}
			}

			TwitterCredentials.ApplicationCredentials = TwitterCredentials.CreateCredentials(
				// "Access_Token"
				credentials[0],
				// "Access_Token_Secret"
				credentials[1],
				// "Consumer_Key"
				credentials[2],
				// "Consumer_Secret"
				credentials[3]
			);

		}

	}
}
