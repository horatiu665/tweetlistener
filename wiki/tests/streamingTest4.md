# New test 29-01-2015
- keyword: dying light
- updates: tweetviewer web app, json saving
- started on 29-01, finishing probably 02-02

## Results
- 130000 tweets were saved, console says 150000 were received, but there was no checking on the amount of missed tweets that twitter reported.
- the dates and times of the tweets were corrupted. this was fixed after the test.
- tweet repeats were more common, with as many as 30 tweets repeating ~200-700 times
- some users posted way more than others, with as many as ~750 tweets from the same user, top 30 all had over 100 tweets each
- there were 12 disconnects which were handled and reconnected, but they were always instantly solved so the missing tweets cannot be due to them.

