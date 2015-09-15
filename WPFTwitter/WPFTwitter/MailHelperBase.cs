using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WPFTwitter
{
	public class MailHelperBase
	{
		Log log;
		Stream stream;

		const string tweetProjectAddress = "tweetprojectku@gmail.com";
		const string fromPassword = "987654321asdf";

		/// <summary>
		/// send spam?
		/// </summary>
		public bool spammingActivated;
		public string windowTitle = "<Untitled>";

		// default at 24 hours = 24*60*60*1000 millis
		public Timer emailTimer = new Timer(1000 * 60 * 60 * 24);

		public MailHelperBase(Log log, Stream stream)
		{
			this.log = log;
			this.stream = stream;

			emailTimer.Elapsed += (s, a) => {
				if (spammingActivated) {
					var message = "The program " + windowTitle + " is running. \n\n"
						+ " Stream " + (stream.StreamRunning ? " running" : " not running") + ". \n\n"
						+ " Spamming at intervals of " + TimeSpan.FromMilliseconds(emailTimer.Interval).ToString();

					SendMail(message, windowTitle + " is running. " + DateTime.Now.ToShortDateString());
				}
			};

			emailTimer.Start();

		}

		public void SendMail(string message, string subject = "Tweet Project update", string destination = "horatiu665@yahoo.com")
		{
			try {
				var fromAddress = new MailAddress(tweetProjectAddress, "Tweet Project");
				var toAddress = new MailAddress(destination, destination);
				string body = message;

				var smtp = new SmtpClient {
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
				};
				using (var mailMessage = new MailMessage(fromAddress, toAddress) {
					Subject = subject,
					Body = body
				}) {
					smtp.Send(mailMessage);
				}
			}
			catch (Exception ex) {
				log.Output("Exception when trying to send e-mail:\n" + ex.ToString());
			}
		}

		/// <summary>
		/// more info:
		/// http://stackoverflow.com/questions/32260/sending-email-in-net-through-gmail
		/// </summary>
		public void SendTestMail()
		{
			try {
				var fromAddress = new MailAddress("tweetprojectku@gmail.com", "Tweet Project");
				var toAddress = new MailAddress("horatiu665@yahoo.com", "Horatiu Roman");
				const string subject = "Tweet Project test mail";
				const string body = "Yo mofo watup niga";

				var smtp = new SmtpClient {
					Host = "smtp.gmail.com",
					Port = 587,
					EnableSsl = true,
					DeliveryMethod = SmtpDeliveryMethod.Network,
					UseDefaultCredentials = false,
					Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
				};
				using (var message = new MailMessage(fromAddress, toAddress) {
					Subject = subject,
					Body = body
				}) {
					smtp.Send(message);
				}
			}
			catch (Exception ex) {
				log.Output("Exception when trying to send e-mail:\n" + ex.ToString());
			}
		}
	}
}
