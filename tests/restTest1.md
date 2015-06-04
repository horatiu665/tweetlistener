# REST test #1
I have created an empty twitter app to test GET and POST requests, as well as the OAuth connection thing.

A useful resource is [the twitter online console app](https://dev.twitter.com/rest/tools/console), which can be used to test various requests. It was only successfully used for the REST API because the Streaming feeds continuous data and is not suited for the app.

## REST Test: GET search/tweets

The GET search/tweets request has been used to test a simple query using #callofduty hashtag.
Important note about GET search/tweets: Not all Tweets will be indexed or made available via the search interface. There must be other ways to get ALL tweets, probably Streaming API.

### Data
Using the console app, logged in as `*horatiu665*`, request: `*search/tweets.json*`, query: `*%23callofduty*`
Full URL of request: 
`https://api.twitter.com/1.1/search/tweets.json?q=%23callofduty`

A bunch of data is returned by the request, some of it is information about the current state of the  app, such as `x-rate-limit-remaining: 179` and `date: Thu, 04 Dec 2014 09:35:34 UTC`.
The rest is contained in an `object { }`, see below.

#### Result outline
The bulk of data is found in the array “statuses”, ommited here for better overview.

`
{
  "statuses": [
    // a bunch of tweets with a lot of data. SEE NEXT SECTION FOR TWEET DATA
    {}, {}, … , {}
  ],
  "search_metadata": {
    "completed_in": 0.043,
    "max_id": 540438876364742660,
    "max_id_str": "540438876364742656",
    "next_results": "?max_id=540434332129562624&q=%23callofduty&include_entities=1",
    "query": "%23callofduty",
    "refresh_url": "?since_id=540438876364742656&q=%23callofduty&include_entities=1",
    "count": 15,
    "since_id": 0,
    "since_id_str": "0"
  }
}
`

#### Tweet data, example tweet
This data can be found in every tweet in the array "statuses". A lot of it seems redundant. 

`
{
      "metadata": {
        "iso_language_code": "en",
        "result_type": "recent"
      },
      "created_at": "Thu Dec 04 09:34:03 +0000 2014",
      "id": 540438876364742660,
      "id_str": "540438876364742656",
      "text": "its been 1 month already for #callofduty #AdvancedWarfare and still its only had 2 lil crappy patches",
      "source": "<a href="http://twitter.com" rel="nofollow">Twitter Web Client</a>",
      "truncated": false,
      "in_reply_to_status_id": null,
      "in_reply_to_status_id_str": null,
      "in_reply_to_user_id": null,
      "in_reply_to_user_id_str": null,
      "in_reply_to_screen_name": null,
      "user": {
        "id": 2361980635,
        "id_str": "2361980635",
        "name": "the smf 1210",
        "screen_name": "thesmf1210",
        "location": "England, UK",
        "profile_location": null,
        "description": "pizza lover, cod fan, gamer, xbox junkie, youtuber, minecrafter, dj, i dont follow 4 follow...",
        "url": "https://t.co/NIWsz26ZOQ",
        "entities": {
          "url": {
            "urls": [
              {
                "url": "https://t.co/NIWsz26ZOQ",
                "expanded_url": "https://www.youtube.com/user/thesmf1210/",
                "display_url": "youtube.com/user/thesmf121…",
                "indices": [
                  0,
                  23
                ]
              }
            ]
          },
          "description": {
            "urls": []
          }
        },
        "protected": false,
        "followers_count": 16,
        "friends_count": 24,
        "listed_count": 0,
        "created_at": "Wed Feb 26 02:17:05 +0000 2014",
        "favourites_count": 6,
        "utc_offset": 0,
        "time_zone": "London",
        "geo_enabled": false,
        "verified": false,
        "statuses_count": 1044,
        "lang": "en",
        "contributors_enabled": false,
        "is_translator": false,
        "is_translation_enabled": false,
        "profile_background_color": "9AE4E8",
        "profile_background_image_url": "http://abs.twimg.com/images/themes/theme16/bg.gif",
        "profile_background_image_url_https": "https://abs.twimg.com/images/themes/theme16/bg.gif",
        "profile_background_tile": false,
        "profile_image_url": "http://pbs.twimg.com/profile_images/531155939915595776/NO0o-xCa_normal.jpeg",
        "profile_image_url_https": "https://pbs.twimg.com/profile_images/531155939915595776/NO0o-xCa_normal.jpeg",
        "profile_link_color": "0084B4",
        "profile_sidebar_border_color": "BDDCAD",
        "profile_sidebar_fill_color": "DDFFCC",
        "profile_text_color": "333333",
        "profile_use_background_image": true,
        "default_profile": false,
        "default_profile_image": false,
        "following": false,
        "follow_request_sent": false,
        "notifications": false
      },
      "geo": null,
      "coordinates": null,
      "place": null,
      "contributors": null,
      "retweet_count": 0,
      "favorite_count": 0,
      "entities": {
        "hashtags": [
          {
            "text": "callofduty",
            "indices": [
              29,
              40
            ]
          },
          {
            "text": "AdvancedWarfare",
            "indices": [
              41,
              57
            ]
          }
        ],
        "symbols": [],
        "user_mentions": [],
        "urls": []
      },
      "favorited": false,
      "retweeted": false,
      "lang": "en"
  }
  `
  
  
