November - December Test Log
====

Gathering was started on 02-11-2015 13:10:52 for five games with release dates as follows:

- Star Wars: Battlefront [November 17]
- Just Cause 3 [December 1]
- Tom Clancy's Rainbow Six Siege [December 1]
- Rollercoaster Tycoon World [December 10]
- Harvest Moon: Seeds of Memories [Likely December, for now set to 25 December (inconsequential until analysis of data)]

The gathering process started with a REST cycle gathering data from as far in the past as possible, and it resulted in ~6000 tweets (note: exact number per game from the REST cycle was not logged but it can be found with some effort by looking up the first X tweets of each game, for which the creation date is ordered anti-chronologically). I will try to log exact numbers more frequently from now on.

I added a column in each table that records the timestamp when the tweet was saved - previously I was only saving the date of the creation of the tweet. The save date allows me to figure out more about the rate of tweet saving - and if there are any suspicious crashes and whatnot.

I checked the tweets gathered in the last days, and it seemed suspicious that only 1 or 2 tweets were saved on 02-11 for Roller coaster tycoon and Harvest Moon - which made me try to perform a REST cycle for the last 2 days, and for each game around 15 tweets were found and added to the database - no duplicates. The same tweets could not be found in the text backup, so the problem was not the database connection, but either the Twitter Stream, or the application stalling and somehow aborting saving the tweets (which seems unlikely because there is no such mechanism in the application, except the ones which throw errors - but it has given no errors). This means that the stream had some problems gathering all the data, or that the data never came through the stream - needs more investigation. 

I will use a program called Process Monitor (procmon.exe) which allows supervision of the various performance data for each application - to monitor the TweetListener for the next period, and see if there are any errors which are not reported, or memory problems, etc., and try to correlate those with the data being gathered next.

Looking a bit closer at the tweets gathered for Harvest Moon, it seems that the hashtag #harvestmoon is used in the context of the actual harvest moon, a natural phenomenon (big red moon in the sky), and not so much about the game.

As mentioned earlier, no errors were found in any of the logs of the application, so it was running as smooth as possible. However, it is still not clear if it wasted resources/discarded any tweets, but this will be more clear after the experiment with Procmon.exe

Taking a closer look at the Star Wars data, it turns out that the 6000 tweets found created on the 01-11-2015 were not all that the program could/should find. Another cycle was run from 30.10 to 31.10 and there were 12000 more tweets found (out of which 6000 english). This means that there is something wrong with the REST gathering, which somehow escaped me so far - probably the process exits before it gets all the tweets, due to either errors that are not reported, or reaching the Rate Limits and not waiting and trying again when they expire. An update to the software is underway.

**An important note is that REST dates do not take time into account, only the actual date. Therefore, if tweets are searched for within the same day, they will never return anything. It is best to use since_id (tweet id) instead of since date - to avoid gathering 1000's of tweets when only looking for the last 5 minutes of downtime or similar.**
