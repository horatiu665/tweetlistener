# TweetListener week test (14-01-2015 - 20-01-2015)
This test will be much more interesting for a few reasons:
- it is performed on localhost, meaning much faster connection between DB and app (but still connected through PHP POST requests)
- it is performed over 2 (if I come Friday) or 6 days (Tuesday) instead of half a day
  - was performed over 2 days, because of server disconnect on Friday
- the tweets have been encoded properly so there is a slim chance that they will be skipped by mistake again (except when saving into DB because I did not encode them from PHP to MySQL)
- slight improvements to logging system and 1 php request instead of 2 like previous test.
- **About maximum database space** (no worries): after querying the database with “show table status”, I found that the InnoDB settings for the tables in the database are max_data_length=0, which means it is “unlimited”. What this means after a small google search is that the maximum size of the database is 64TB. Yes, that is a lot. Anyway, we’ll see if this limit is really what happens for us too, or if we have some different settings that do not allow us yet to reach this level of insanity. The amount of free space on this computer is only 31 GB on C:\, so depending on the amount of callofduty players in the weekend, we might run out of space or not. (post-mortem: 5633 tweets and 10500 words occupy in total 4.4 MB. to run out of space we would need to have 1000x that, so 5.6 mil tweets and 10.5 mil words)

## Intermediary check
On Friday, 16-01-2015, 10:00 I arrived to check the progress of the data gathering. It seems that there were 5000+ tweets found, with no tweets lost, and no crashes or disconnects. The database weighs a little over 3 MB, which fits on 3 floppy disks.

Below is a useful query for finding the number of duplicate entries in a database.

```
SELECT
    id, tweet, COUNT(*)
FROM
    tweets
GROUP BY
    tweet
HAVING 
    COUNT(*) > 1
ORDER BY COUNT(*)  DESC
```

After running this query, the interesting result is that there were a few tweets that repeated themselves 700, 200, 119, 97 times respectively, and so on until others that only repeated twice or three times. Still these repetitions could be taken into account at a later point, for two reasons:
- limit the storage requirements even more
- ability to count unique data instead of repeated data
The program will be left running until Tuesday, 20-01-2015, in hope that errors might occur and be handled (to prepare for heavier tests later on)

## Test results 20-01-2015
Tweet stream disconnected on 16-01-2015 at 16:48:23 for unclear reasons, and reconnects were attempted by the disconnect handler. 
There were 3 different errors triggered within 5 seconds, the second error occurring 3 times, yielding 5 disconnects in total.

- First error: 
   ``` 
   Exception: System.ArgumentException: Stream was not readable.
   at System.IO.StreamReader..ctor(Stream stream, Encoding encoding, Boolean detectEncodingFromByteOrderMarks, Int32 bufferSize, Boolean leaveOpen)
   at System.IO.StreamReader..ctor(Stream stream, Encoding encoding)
   at Tweetinvi.Streams.Helpers.StreamResultGenerator.<InitWebRequest>d__d.MoveNext()
```

- Second error (appeared 3 times):

```
Exception: System.ArgumentNullException: Value cannot be null.
Parameter name: source
   at AsyncExtensions.ReadLineAsync(TextReader source)
   at Tweetinvi.Streams.Helpers.StreamResultGenerator.<StartStreamAsync>d__7.MoveNext()
```

- Third error:

```
Exception: System.NotSupportedException: The stream does not support concurrent IO read or write operations.
   at Microsoft.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
   at Microsoft.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccess(Task task)
   at Microsoft.Runtime.CompilerServices.TaskAwaiter`1.GetResult()
   at Tweetinvi.Streams.Helpers.StreamResultGenerator.<StartStreamAsync>d__7.MoveNext()
```

Final log shows stream successfully start at 16:49:45, 2 minutes after first disconnect, after all the errors, and that is the last entry in the log = no tweets were received. If the stream is indeed started, that could mean a few things
- Twitter banned the app for excessive reconnecting, as warned in the docs
- Connection was made with some missing parameters, being reset somehow through repeated reconnects
- nobody tweeted about callofduty since 16-01-2015 (false, there was at least 1 tweet 6 hours ago which should have been captured)

## Some analysis of results
- Repetitive tweets must be dealt with (700/5000 could ruin results)
- Words must be split more accurately, take into account punctuation and such
  - perhaps follow some guide online for word parsing
- streaming = too many results when simple improvement ideas can come from few results. REST must be implemented next.
