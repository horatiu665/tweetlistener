# Query expansion test
- starting hashtag: `#splatoon`
- date: `19-05-2015 12:28:44` to `06-06-2015 14:51`
- expansion is performed at 24h intervals, naively by taking top 10% of  hashtags after being ranked by total count in gathered tweet population

## Intermediary check
- on `23-05-2015 12:14:00` it was found that the program did not restart the stream after expanding queries, though expansion occurred at the right moments and displayed discovered hashtags.
- a manual expansion was triggered and the stream restarted - which made the amount of tweets skyrocket from 10k to 180k and crash the program after some hours at 2 am.
- the program was restarted on `26-05-2015 9:27:17` with the original tag and no expansion to avoid further crashing

## results
### manual expansion
The tags found by the manual expansion were somewhat relevant to #splatoon but many were generalistic, such as #uk, #gaming, #nintendo, #WiiU, #xbox, ... . Others were about other games such as #bloodborne, #evolve, #witcher, most likely due to people talking about "which one to play" and such.

Some of the generalistic tags were removed but they were far too many to properly remove by hand, and removal of the most obvious ones still did not avoid the crash (probably postponed it though).

### twitter technicalities
There were a bunch of rate limit messages with values between 1 and 65, therefore not many tweets were lost.

There were also a bunch of stream reconnects with a simple REST gathering mechanism in between disconnect and connect, but which never seemed to yield any results, possibly because there were no tweets in the short amount of time, but possibly because they are just not returned in time by Twitter.

An error message which happened very often is:

```
Exception: --- Date : <date here, example: 03-06-2015 11:18:11>
URL: https://stream.twitter.com/1.1/statuses/filter.json?track=%23splatoon
Code: -1
Error documentation description: 

```

At some points, it happened in bursts of ~4 per second for a few minutes. Could be a few things: internet disconnect, rate limiting, twitter problems, etc.

The stream reconnected afterwards though, probably the problem went away by itself. Would be nice to investigate stuff such as internet connection and graph the results.

The stupid thing about the REST gathering which happens between disconnects is that it might not work if the disconnect also affects REST, such as connection failure or whatnot. The REST algorithm should instead run when the stream successfully starts and the first new tweet is received, to make sure that past tweets CAN be received. And only then the REST should run between the last disconnect and the first tweet received.

### About expansion

The main lesson is that naive expansion on 10% is way too naive (simply cannot be handled) - because of too many tweets received by simple expansion, and especially irrelevant ones. It might also be that more clever methods such as Efron's will still capture weird irrelevant tags and miss relevant ones, but since it bases its expansion on factors such as language model, rank and hashtag association, it can avoid many of the problems. Useful visualization tools can be made prior to implementing the clever system, as intermediary steps, for example hashtag association.

### Relevance and frequency of hashtags

A relevant hashtag to #splatoon is #SplatoonGlobalTestfire, which seems to relate to a beta-testing thing that Nintendo did prior to release of the game. People were tweeting about it while others were expecting the game to release. The tag was found few times though, especially compared to #gaming and such others. It is therefore interesting to note that very common hashtags can actually cover-up useful ones, so an idea would be to not expand on the extremely common tags, only on less common ones that seem to be relevant.

