## Calculations on the amount of data gathered
Using REST API, in the console, the count of tweets is set between 1 and 100 for any request, default 15. The query limit is *450* every *15 minutes* for application based auth, this results in maximum *45000* statuses coming from different queries, of which it is possible that many are the same. This can be avoided by cleverly using `max_id` and `since_id` to avoid the repetition of returned data, therefore this means that we can get *45000* different statuses every 15 minutes provided the software is capable of processing the data and requesting it fast enough.

Simple math shows that we can read through `45000 * 4 = 180000` tweets per hour, `* 24 = 4.320.000 tweets per day`, ` * 30 days = 129.600.000 per month`, just by using the REST API and 1 app.

The amount of raw data in the 15 generic tweets found by the call of duty query can be stored, on average, in `4584 bytes per tweet`, or `68773 bytes per 15 tweets`. An hour's worth of tweets can be therefore stored in `4584 * 180000 = 825120000 bytes = 787 MB`. A day’s worth of tweets is stored in `18 gb`, and a month of tweet data is `553 gb`. Quite a handful, therefore some of the redundant data should not be stored, or smaller samples must be used instead.

### How much data can be read using REST?
Tweets gathered using GET search/tweets:
- 180.000 tweets per hour
- 4.320.000 tweets per day
- 129.600.000 per month

### How much storage is needed for this data? (raw, no optimization)
Storage requirements:
- 4584 bytes per tweet
- 787 MB per hour
- 18 GB per day
- 553 GB per month

### How much storage is needed for minimally optimized data? (shibabbidy optimization)
(only redundant parts omitted, still taking into account the attribute names such as “id” and “text” and “in_reply_to_screen_name” and such - but no proper research was made on optimizing size, hence the term shibabbidy. see section about optimizations for more strategies)

Omitted data: 
- user profile data (profile picture, profile colors)
- entities data (hashtags) - they can be found by parsing the tweet itself
Did specifically not omit:
- id or id_str - they are different even though they should be the same - something to do with ids exceeding 32 bits and needing to be stored as strings instead of numbers
- entities URLs, because the tweet contains short versions: `http://t.co/V34DKG4MCD` instead of `http://youtu.be/TzTMTGWX7m4` - optionally, replace URL in tweet with expanded URL in entities, to further minimize storage req.

Storage requirements:
- 2192 bytes per tweet
- 376 MB per hour
- 8.8 GB per day
- 264 GB per month
