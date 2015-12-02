using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetListener2.Systems
{
    public class BatchManager
    {
        public BatchManager()
        {


        }

        /// <summary>
        /// Reads the batch commands and initializes systems referenced by the batch
        /// </summary>
        public void ReadBatch()
        {

        }

        /// <summary>
        /// Serializes the current settings in the referenced systems, and creates a batch string which would generate a TweetListener with identical settings to the current one
        /// </summary>
        public string WriteBatch()
        {
            return "start LOL";
        }
    }
}
