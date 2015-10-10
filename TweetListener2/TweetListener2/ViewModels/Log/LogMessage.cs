using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.ViewModels
{
    public class LogMessage
    {
        public string Time
        {
            get; private set;
        }

        public string Message
        {
            get; private set;
        }

        public LogMessage(string message)
        {
            Time = DateTime.Now.ToString();
            Message = message;
        }

        public override string ToString()
        {
            return Time + " " + Message;
        }

    }
}
