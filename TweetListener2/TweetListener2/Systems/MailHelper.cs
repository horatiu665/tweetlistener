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
    public class MailHelper
    {
        private Log log;
        private Stream stream;

        const string tweetProjectAddress = "tweetprojectku@gmail.com";
        const string fromPassword = "987654321asdf";

        public bool emailSpammer;

        DateTime streamStarted, streamStopped, tweetReceived;

        // default at 6 hours = 6*60*60*1000 millis
        public Timer emailTimer = new Timer(1000 * 60 * 60 * 6);
        private bool sentMailForLatestDisconnect = false;

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

        public Stream Stream
        {
            get
            {
                return stream;
            }

            set
            {
                stream = value;
            }
        }

        public MailHelper(Log log, Stream stream)
        {
            this.Log = log;
            this.Stream = stream;

            stream.stream.StreamStarted += OnStreamStarted;
            stream.sampleStream.StreamStarted += OnStreamStarted;
            stream.stream.StreamStopped += OnStreamStopped;
            stream.sampleStream.StreamStopped += OnStreamStopped;
            stream.stream.MatchingTweetReceived += OnTweetReceived;
            stream.sampleStream.TweetReceived += OnSampleTweetReceived;

            emailTimer.Elapsed += (s, a) => {
                var message = "Stream was disconnected since " + streamStopped.ToString() + " for more than " + TimeSpan.FromMilliseconds(emailTimer.Interval).ToString() + ".";
                message += "\n\n" + "Query for the stream:\n";

                if (stream.stream.StreamState == Tweetinvi.Core.Enum.StreamState.Resume) {
                    foreach (var t in stream.stream.Tracks) {
                        message += t.Key + ", ";
                    }
                } else {
                    message += "sample stream...";
                }

                message += "\n\n" + "Please fix!";
                SendMail(message);
                sentMailForLatestDisconnect = true;

            };

        }

        private void OnSampleTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.TweetReceivedEventArgs e)
        {
            tweetReceived = DateTime.Now;
            emailTimer.Stop();
            sentMailForLatestDisconnect = false;
        }

        private void OnTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
        {
            tweetReceived = DateTime.Now;
            emailTimer.Stop();
            sentMailForLatestDisconnect = false;
        }

        private void OnStreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
        {
            streamStopped = DateTime.Now;
            if (!emailTimer.Enabled) {
                emailTimer.Start();
            }
        }

        private void OnStreamStarted(object sender, EventArgs e)
        {
            streamStarted = DateTime.Now;
            if (sentMailForLatestDisconnect) {
                // stream was disconnected for longer than allowed, and there were no tweets received in the meanwhile.
                // But now the stream has started again.
                // what if there are no tweets received for a long time while the stream is functioning?

                // do nothing here, consider stream inactive until tweets are found. 
                // if no tweets are received in X interval, it is just as bad as being disconnected.
                // if that result is expected, the interval should just be increased, or maybe it shouldn't send emails.
            }
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
                Log.Output("Exception when trying to send e-mail:\n" + ex.ToString());
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
                Log.Output("Exception when trying to send e-mail:\n" + ex.ToString());
            }
        }
    }
}

