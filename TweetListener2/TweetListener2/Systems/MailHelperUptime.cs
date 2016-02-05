using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace TweetListener2.Systems
{
    public class MailHelperUptime
    {
        const string tweetProjectAddress = "tweetprojectku@gmail.com";
        const string fromPassword = "987654321asdf";
        
        // default at 24 hours = 24*60*60*1000 millis
        public Timer emailTimer = new Timer();

        public List<string> destinations = new List<string>();

        public string message = "TweetListener running.";

        public MailHelperUptime(params string[] destinations)
        {
            this.destinations.AddRange(destinations);

            var tomorrowAt7am = DateTime.Now.AddSeconds(60 - DateTime.Now.Second).AddMinutes(60 - DateTime.Now.Minute).AddHours(24 - DateTime.Now.Hour + 7);
            //var tomorrowAt7am = DateTime.Now.AddSeconds(5);

            emailTimer.Interval = tomorrowAt7am.Subtract(DateTime.Now).TotalMilliseconds;

            emailTimer.Elapsed += (s, a) => {
                SendMail();
                emailTimer.Stop();
                emailTimer.Interval = 1000 * 60 * 60 * 24;
                emailTimer.Start();
            };
            
            emailTimer.Start();
        }

        public void SendMail()
        {
            try {
                var fromAddress = new MailAddress(tweetProjectAddress, "Tweet Project");
                var subject = "TweetListener running";
                string body = message;

                var smtp = new SmtpClient {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                
                using (var mailMessage = new MailMessage() {
                    Subject = subject,
                    Body = body
                }) {
                    mailMessage.From = fromAddress;
                    foreach (var d in destinations) {
                        mailMessage.To.Add(d);
                    }
                    
                    smtp.Send(mailMessage);
                }
            }
            catch (Exception ex) {
                // do nothing. Not receiving mail means that TweetListener must be restarted.

            }
        }
        
    }
}

