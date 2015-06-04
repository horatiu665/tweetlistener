# New test 03-02-2015
- keyword: dying light
- dates are saved properly
- tweets are also encoded properly in the database, and there are some problems when displaying (japanese) tweets, but not as they are stored.
- The program will run until 05-02-2015: 45000 tweets and going strong. the program will continue to run until 10-02-2015
out of 45000 not one was off topic containing “dying of the light”
- program crashed on 06-02-2015 at 00:20:53 because Twitter probably disconnected stream. the crash log states: 

  ```
  Exception: System.ArgumentException: Stream was not readable.
   at System.IO.StreamReader..ctor(Stream stream, Encoding encoding, Boolean detectEncodingFromByteOrderMarks, Int32 bufferSize, Boolean leaveOpen)
   at System.IO.StreamReader..ctor(Stream stream, Encoding encoding)
   at Tweetinvi.Streams.Helpers.StreamResultGenerator.<InitWebRequest>d__d.MoveNext()
   ```
- still have 2.5 days of tweets to evaluate: from `03-02-2015 15:13:58` to `06-02-2015 00:20:53`
