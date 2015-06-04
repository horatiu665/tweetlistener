# Basic Twitter API

Searching is done using Twitter API v1.1, which requires some sort of authentication, so search queries cannot be performed by just anyone and with some limits.

[There is a freemium service] (http://tweetreach.com/) that allows limited queries, it was recommended in [this tutorial] (https://unionmetrics.com/resources/how-to-use-advanced-twitter-search-queries/) as a test when creating queries to see if the results are valid/similar/etc. The tutorial offers helpful tips for searching various things such as by language, by location, and/or/not, by account.

Also obvious choice for testing queries is [the actual twitter search function] (https://twitter.com/search-home) though it behaves slightly different than the queries using REST or Streaming.

Another useful service is the [console dev app](https://dev.twitter.com/rest/tools/console) which allows queries to be performed with actual results shown in return.

## REST API

Service requiring authentication for querying data, and having limitations of how much data can be returned. Authentication is based on OAuth.

### How to auth
long story at:
https://dev.twitter.com/oauth 

#### Auth user-based
users sign in to application, using their twitter account. This allows them to post statuses and do other stuff.
Simple, 1 page short explanation here:
https://dev.twitter.com/oauth/3-legged 
#### Auth application-based
No matter what user, the application can access some information from twitter, such as queries about statuses and stuff like that. For our purposes, application-based is probably the way to go.

limits are global, per application, per time window. example: 20 queries allowed every 15 minutes. If limit is exceeded, queries return

`{ 
"errors": 
  [
    { 
      "code": 88,
      "message": "Rate limit exceeded" 
    }
  ]
}`

find limit by querying GET application/rate_limit_status. more info:
https://dev.twitter.com/rest/reference/get/application/rate_limit_status 

### How to make a query
Simple: use twitter https://twitter.com/search-home default search, copy URL and use in application.
Replace in the URL “https://twitter.com/search” with “https://api.twitter.com/1.1/search/tweets.json”
“Also note that the search results at twitter.com may return historical results while the Search API usually only serves tweets from the past week.”
#### Query limits
The limits for how many queries per 15 minutes for REST API can be found here:
https://dev.twitter.com/rest/public/rate-limits 
## Streaming API
Streaming provides real-time global tweet data and is “suitable for following specific users or data, and data mining” - Streaming overview docs.

### Auth
Streaming connections also need to be authentified. App based authentication is very easy, can be done by following twitter’s service, using Customer Key, Access Key and Tokens and stuff. https://apps.twitter.com/app/7552745/show 
### Connection rules and bans
If connection fails, must attempt to reconnect in a specific way, as described [here] (https://dev.twitter.com/streaming/overview/connecting).

It is specified that a fallback to using REST is required for scenarios when reconnection is not possible (see headline Connection Churn).

To avoid blacklisting: “However, it is essential to stop further connection attempts for a few minutes if a HTTP 420 response is received.”

Once applications establish a connection to a streaming endpoint, they are delivered a feed of Tweets, without needing to worry about polling or REST API rate limits.

### Streaming limits
Streaming is virtually unlimited, as shown by the test with the console app, described in the Testing/Streaming section.

There are however limits on various parts of streaming.
#### Streaming/firehose
The firehose is the full twitter real-time data stream. It is only accessible through external providers, for shitton of money.

Providers usually have access to historical data as well as real-time data, because they store the data on their servers and feed it to the users as they request it, not requiring a user to store it themselves. Some offer data visualization and other nifty tools. None of them have clear pricing options, and many of them offer multiple sites’ data, such as Tumblr, Facebook, Vine, etc.

I contacted one of them called [DataSift](https://dev.twitter.com/streaming/overview/connecting) asking for price and stuff. They replied with astronomical prices for a yearly subscription to historical data:  *“Average client spend is $200,000/year. With a bare bones subscription you could be as low as $65,000/year + data fees. Trial access to our data platform is currently available on our website with a developer's trial.  However some data sources like Twitter, NewsCred, LexisNexis, etc are not available through the trial.”* - DataSift dude

#### Streaming/sample
The sample “garden hose” is limited to 1% of all firehose data, but without filtering. The random sample returned is distributed by some Twitter rules, in the same way for all connections to the sample stream. (it is not a fully random sampling).

#### Streaming/filtered
The filtered stream is limited to 1% of all firehose data, but if the filter does not exceed this 1% size limit, it returns all the results. If it exceeds the limit of data required, it tells how much of this data was omitted via limit JSON messages.

Useful discussion about limit of filters: [what does the track number mean?](https://twittercommunity.com/t/rate-limit-in-streaming-api-what-does-the-track-number-mean/9560)

Limit of tracks (= keywords = filters) is **400 per stream connection**. Info is given about the amount of tweets not checked since the connection was made (and not about the amount of matched tweets) as follows:
##### Limit notices (limit)
These messages indicate that a filtered stream has matched more Tweets than its current rate limit allows to be delivered. Limit notices contain a total count of the number of undelivered Tweets since the connection was opened, making them useful for tracking counts of track terms, for example. Note that the counts do not specify which filter predicates undelivered messages matched.

`
{
  "limit":{
    "track":1234
  }
}
`

Streaming messages must be handled properly to avoid ban.

[This link in the twitter docs](This link in the twitter docs) provides info about public stream messages that can be received from a stream. They are usually problems that need fixing.

